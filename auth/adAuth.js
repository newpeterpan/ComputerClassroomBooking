const ActiveDirectory = require('activedirectory2');
const config = {
    url: 'ldap://your.domain.com',
    baseDN: 'dc=domain,dc=com',
    username: 'service_account@domain.com',
    password: 'service_account_password'
};

const ad = new ActiveDirectory(config);

async function authenticateUser(username, password) {
    return new Promise((resolve, reject) => {
        ad.authenticate(username + '@domain.com', password, (err, auth) => {
            if (err) {
                console.log('AD 驗證錯誤:', err);
                reject(err);
                return;
            }
            
            if (auth) {
                // 驗證成功後獲取用戶資訊
                ad.findUser(username, (err, user) => {
                    if (err) {
                        reject(err);
                        return;
                    }
                    resolve(user);
                });
            } else {
                reject(new Error('驗證失敗'));
            }
        });
    });
}

module.exports = { authenticateUser }; 