const express = require('express');
const router = express.Router();
const { authenticateUser } = require('../auth/adAuth');
const jwt = require('jsonwebtoken');
const db = require('../database');

router.post('/login', async (req, res) => {
    try {
        const { username, password } = req.body;
        
        // AD 驗證
        const adUser = await authenticateUser(username, password);
        
        if (adUser) {
            // 檢查或創建本地用戶記錄
            let user = await db.query(
                'SELECT * FROM users WHERE ad_username = ?',
                [username]
            );
            
            if (!user.length) {
                // 首次登入，創建用戶記錄
                await db.query(`
                    INSERT INTO users (username, email, ad_username, role, department)
                    VALUES (?, ?, ?, 'teacher', ?)
                `, [adUser.displayName, adUser.mail, username, adUser.department]);
                
                user = await db.query(
                    'SELECT * FROM users WHERE ad_username = ?',
                    [username]
                );
            }
            
            // 更新最後登入時間
            await db.query(
                'UPDATE users SET last_login = CURRENT_TIMESTAMP WHERE id = ?',
                [user[0].id]
            );
            
            // 記錄登入日誌
            await db.query(`
                INSERT INTO login_logs (user_id, login_status, ip_address, user_agent)
                VALUES (?, 'success', ?, ?)
            `, [user[0].id, req.ip, req.headers['user-agent']]);
            
            // 生成 JWT token
            const token = jwt.sign(
                { userId: user[0].id, role: user[0].role },
                process.env.JWT_SECRET,
                { expiresIn: '8h' }
            );
            
            res.json({ token, user: user[0] });
        }
    } catch (error) {
        console.error('登入錯誤:', error);
        res.status(401).json({ error: '登入失敗' });
    }
});

module.exports = router; 