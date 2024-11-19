-- 教室資料表
CREATE TABLE classrooms (
    id INT PRIMARY KEY AUTO_INCREMENT,
    room_number VARCHAR(10) NOT NULL,
    capacity INT NOT NULL,
    description TEXT,
    status ENUM('available', 'maintenance', 'reserved') DEFAULT 'available'
);

-- 使用者資料表
CREATE TABLE users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    ad_username VARCHAR(50) NOT NULL UNIQUE,
    role ENUM('admin', 'teacher', 'staff') NOT NULL,
    department VARCHAR(50),
    last_login TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_ad_username (ad_username)
);

-- 預約資料表
CREATE TABLE reservations (
    id INT PRIMARY KEY AUTO_INCREMENT,
    classroom_id INT,
    user_id INT,
    start_time DATETIME NOT NULL,
    end_time DATETIME NOT NULL,
    purpose TEXT,
    status ENUM('pending', 'approved', 'rejected', 'cancelled') DEFAULT 'pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (classroom_id) REFERENCES classrooms(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- 新增登入記錄表
CREATE TABLE login_logs (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT,
    login_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    login_status ENUM('success', 'failed') NOT NULL,
    ip_address VARCHAR(45),
    user_agent TEXT,
    FOREIGN KEY (user_id) REFERENCES users(id)
); 