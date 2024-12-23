﻿USE master
GO
CREATE DATABASE UniEXETask
GO
USE UniEXETask
GO

-- Tạo bảng Campus
CREATE TABLE CAMPUS (
    campus_id INT PRIMARY KEY IDENTITY(1,1),
    campus_code NVARCHAR(50) NOT NULL,
    campus_name NVARCHAR(100) NOT NULL,
    location NVARCHAR(100) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng SUBJECT
CREATE TABLE SUBJECT (
    subject_id INT PRIMARY KEY IDENTITY(1,1),
    subject_code NVARCHAR(50) NOT NULL,
    subject_name NVARCHAR(50) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng Role
CREATE TABLE ROLE (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng FEATURE
CREATE TABLE FEATURE (
    feature_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng Permission
CREATE TABLE PERMISSION (
    permission_id INT PRIMARY KEY IDENTITY(1,1),
	feature_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
	FOREIGN KEY (feature_id) REFERENCES FEATURE(feature_id),
);

-- Tạo bảng User
CREATE TABLE [USER] (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(255),
    email NVARCHAR(100) NOT NULL UNIQUE,
    phone NVARCHAR(20),
	avatar NVARCHAR(max),
    campus_id INT NOT NULL,
    role_id INT NOT NULL,
    FOREIGN KEY (campus_id) REFERENCES CAMPUS(campus_id),
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id),
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng ROLE_PERMISSION
CREATE TABLE ROLE_PERMISSION (
    role_id INT NOT NULL,
    permission_id INT NOT NULL,
    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES Role(role_id),
    FOREIGN KEY (permission_id) REFERENCES PERMISSION(permission_id)
);

-- Tạo bảng MENTOR
CREATE TABLE MENTOR (
    mentor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	specialty NVARCHAR(250) NOT NULL,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng STUDENT
CREATE TABLE STUDENT (
    student_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	lecturer_id INT NOT NULL,
    student_code NVARCHAR(10) NOT NULL UNIQUE,
    major NVARCHAR(250) NOT NULL,
	subject_id INT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (user_id) REFERENCES [USER](user_id),
	FOREIGN KEY (lecturer_id) REFERENCES MENTOR(mentor_id),
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng GROUP
CREATE TABLE [GROUP] (
    group_id INT PRIMARY KEY IDENTITY(1,1),
	group_name NVARCHAR(250) NOT NULL,
	subject_id INT NOT NULL,
	hasMentor BIT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
	status NVARCHAR(20) CHECK (status IN ('Initialized', 'Eligible', 'Approved', 'Overdue')) NOT NULL,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id),
	isDeleted BIT NOT NULL DEFAULT 0,
);

-- Tạo bảng CHAT_GROUP
CREATE TABLE CHAT_GROUP (
    chat_group_id INT PRIMARY KEY IDENTITY(1,1),
    chat_group_name NVARCHAR(50) NOT NULL,
	chat_group_avatar nvarchar(255),
    created_date DATETIME DEFAULT GETDATE() NOT NULL,
	created_by INT NOT NULL,
	owner_id INT NOT NULL,
	group_id INT NULL,
	latest_activity DATETIME DEFAULT GETDATE() NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Personal', 'Group')) NOT NULL,
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
	FOREIGN KEY (created_by) REFERENCES [USER](user_id),
	FOREIGN KEY (owner_id) REFERENCES [USER](user_id)
);

-- Tạo bảng CHAT_MESSAGE
CREATE TABLE CHAT_MESSAGE (
    message_id INT PRIMARY KEY IDENTITY(1,1),
    chat_group_id INT NOT NULL,
	user_id INT NOT NULL,
	message_content NVARCHAR(4000) NOT NULL,
	send_datetime DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (chat_group_id) REFERENCES CHAT_GROUP(chat_group_id),
    FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng USER_CHAT_GROUP
CREATE TABLE USER_CHAT_GROUP (
    user_id INT NOT NULL,
    chat_group_id INT NOT NULL,
    PRIMARY KEY (user_id, chat_group_id),
    FOREIGN KEY (user_id) REFERENCES [USER](user_id),
    FOREIGN KEY (chat_group_id) REFERENCES CHAT_GROUP(chat_group_id)
);

-- Tạo bảng TIMELINE
CREATE TABLE TIMELINE (
    timeline_id INT PRIMARY KEY IDENTITY(1,1),
    timeline_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	subject_id INT NOT NULL,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng TOPIC
CREATE TABLE TOPIC (
    topic_id INT PRIMARY KEY IDENTITY(1,1),
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL
);

-- Tạo bảng PROJECT
CREATE TABLE PROJECT (
    project_id INT PRIMARY KEY IDENTITY(1,1),
	group_id INT NOT NULL,
	topic_id INT NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	subject_id INT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
	status NVARCHAR(20) CHECK (status IN ('In_Progress', 'Completed')) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id),
	FOREIGN KEY (topic_id) REFERENCES TOPIC(topic_id),
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng PROJECT_PROGRESS
CREATE TABLE PROJECT_PROGRESS (
    project_progress_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
    updated_date DATETIME NOT NULL,
    note NVARCHAR(250),
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id)
);

-- Tạo bảng TASK
CREATE TABLE TASK (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    task_name NVARCHAR(50) NOT NULL,
	description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Not_Started', 'In_Progress', 'Completed', 'Overdue')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng TASK_PROGRESS
CREATE TABLE TASK_PROGRESS (
    task_progress_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
    updated_date DATETIME NOT NULL,
    note NVARCHAR(250),
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (task_id) REFERENCES TASK(task_id)
);

-- Tạo bảng TASK_ASSIGN
CREATE TABLE TASK_ASSIGN (
    task_assign_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    student_id INT NOT NULL,
	assigned_date DATETIME NOT NULL,
	FOREIGN KEY (task_id) REFERENCES TASK(task_id),
	FOREIGN KEY (student_id) REFERENCES STUDENT(student_id)
);

-- Tạo bảng TASK_DETAIL
CREATE TABLE TASK_DETAIL (
    task_detail_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    task_detail_name NVARCHAR(250) NOT NULL, -- Mô tả công việc cụ thể trong task
	progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
	isCompleted BIT NOT NULL DEFAULT 0,
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (task_id) REFERENCES TASK(task_id)
);

-- Tạo bảng DOCUMENT
CREATE TABLE DOCUMENT (
    document_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    name NVARCHAR(250) NOT NULL,
    type NVARCHAR(20) CHECK (type IN (
        'DOC',
        'DOCX',
        'XLS',      
        'XLSX',
        'PDF',      
        'TXT',      
        'JPG',      
        'JPEG',
        'PNG',
        'ZIP',      
        'RAR'
    )) NOT NULL,
    url NVARCHAR(250) NOT NULL,
	modified_by INT,
	modified_date DATETIME,
    upload_by INT NOT NULL,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	FOREIGN KEY (modified_by) REFERENCES [USER](user_id),
	FOREIGN KEY (upload_by) REFERENCES [USER](user_id),
    isDeleted BIT NOT NULL DEFAULT 0
);


-- Tạo bảng MENTOR_GROUP
CREATE TABLE MENTOR_GROUP (
    group_id INT NOT NULL,
    mentor_id INT NOT NULL,
    PRIMARY KEY (group_id, mentor_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

-- Tạo bảng MEETING_SCHEDULE
CREATE TABLE MEETING_SCHEDULE (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
	meeting_schedule_name NVARCHAR(250) NOT NULL,
    group_id INT NOT NULL,
    mentor_id INT NOT NULL,
	location NVARCHAR(MAX) NOT NULL,
	meeting_date DATETIME NOT NULL,
	duration INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Offline', 'Online')) NOT NULL,
	content NVARCHAR(250) NOT NULL,
	url NVARCHAR(250),
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
	FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

-- Tạo bảng GROUP_MEMBER
CREATE TABLE GROUP_MEMBER (
    group_id INT NOT NULL,
    student_id INT NOT NULL,
	role NVARCHAR(20) CHECK (role IN ('Leader', 'Member')) NOT NULL,
    PRIMARY KEY (group_id, student_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (student_id) REFERENCES STUDENT(student_id)
);

-- Tạo bảng NOTIFICATION
CREATE TABLE NOTIFICATION (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
	message NVARCHAR(250) NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Info', 'Group_Request')) NOT NULL,
	created_at DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Sent', 'Read')) NOT NULL,
	FOREIGN KEY (sender_id) REFERENCES [USER](user_id),
	FOREIGN KEY (receiver_id) REFERENCES [USER](user_id)
);

-- Tạo bảng GROUP_INVITE
CREATE TABLE GROUP_INVITE (
    group_id INT NOT NULL,
    notification_id INT NOT NULL,
	inviter_id INT NOT NULL,
    invitee_id INT NOT NULL,
	created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Pending', 'Accepted', 'Rejected', 'Expired')) NOT NULL,
    PRIMARY KEY (group_id, notification_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (notification_id) REFERENCES NOTIFICATION(notification_id)
);

-- Tạo bảng EVENT
CREATE TABLE WORKSHOP (
    workshop_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	location NVARCHAR(MAX) NOT NULL,
	reg_url NVARCHAR(MAX) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Not_Started', 'In_Progress', 'Completed')) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng MILESTONE để định nghĩa các mốc đánh giá
CREATE TABLE MILESTONE (
    milestone_id INT PRIMARY KEY IDENTITY(1,1),
    milestone_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(250),
	percentage FLOAT NOT NULL,
	subject_id INT NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
    created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng CRITERIA
CREATE TABLE CRITERIA (
    criteria_id INT PRIMARY KEY IDENTITY(1,1),
    criteria_name NVARCHAR(250) NOT NULL,
	description NVARCHAR(250) NOT NULL,
    percentage FLOAT NOT NULL,
	milestone_id INT NOT NULL,
	created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (milestone_id) REFERENCES MILESTONE(milestone_id)
);

-- Tạo bảng PROJECT_SCORE
CREATE TABLE PROJECT_SCORE (
    project_score_id INT PRIMARY KEY IDENTITY(1,1),
    criteria_id INT NOT NULL,
	project_id INT NOT NULL,
    score FLOAT NOT NULL,
	comment NVARCHAR(250) NOT NULL,
    scored_by INT NOT NULL, -- ID của người chấm điểm (mentor)
    scoring_date DATETIME DEFAULT GETDATE() NOT NULL,
	FOREIGN KEY (criteria_id) REFERENCES CRITERIA(criteria_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	FOREIGN KEY (scored_by) REFERENCES [USER](user_id),
	CONSTRAINT CHK_ProjectScore CHECK (score >= 0 AND score <= 10)
);

-- Tạo bảng MEMBER_SCORE để lưu điểm của từng thành viên theo milestone
CREATE TABLE MEMBER_SCORE (
    member_score_id INT PRIMARY KEY IDENTITY(1,1),
    student_id INT NOT NULL,
    project_id INT NOT NULL,
    milestone_id INT NOT NULL,
    score FLOAT NOT NULL,
    comment NVARCHAR(250),
    scored_by INT NOT NULL, -- ID của người chấm điểm (mentor)
    scoring_date DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (student_id) REFERENCES STUDENT(student_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (milestone_id) REFERENCES MILESTONE(milestone_id),
    FOREIGN KEY (scored_by) REFERENCES MENTOR(mentor_id),
    CONSTRAINT CHK_MemberScore CHECK (score >= 0 AND score <= 10)
);

-- Tạo bảng REFRESH_TOKEN
CREATE TABLE REFRESH_TOKEN (
    token_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
    token NVARCHAR(MAX) NOT NULL,
    expires DATETIME NOT NULL,
	created  DATETIME DEFAULT GETDATE() NOT NULL,
	revoked  DATETIME NOT NULL,
	status BIT NOT NULL DEFAULT 1,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng REG_TOPIC_FORM
CREATE TABLE REG_TOPIC_FORM (
    reg_topic_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL,
	rejection_reason NVARCHAR(MAX) NULL,
	status BIT NOT NULL,
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng REG_MEMBER_FORM
CREATE TABLE REG_MEMBER_FORM (
    reg_member_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
	description NVARCHAR(250) NOT NULL,
	status BIT NOT NULL,
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng TOPIC_FOR_MENTOR
CREATE TABLE TOPIC_FOR_MENTOR (
    topic_for_mentor_id INT PRIMARY KEY IDENTITY(1,1),
    mentor_id INT NOT NULL,
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL,
	isRegistered BIT NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

CREATE TABLE CONFIG_SYSTEM (
    config_id INT PRIMARY KEY IDENTITY(1,1),
	config_name NVARCHAR(250) NOT NULL,
	number INT,
	start_date DATETIME
);


-- Thêm dữ liệu mẫu cho bảng Campus
INSERT INTO CONFIG_SYSTEM (config_name, number, start_date)
VALUES 
('MAX_REG_TOPIC', 5, NULL),
('EDIT_MENTOR', NULL, '2024-12-15'),
('MIN_MEMBER_EXE101', 4, NULL),
('MAX_MEMBER_EXE101', 6, NULL),
('MIN_MEMBER_EXE201', 6, NULL),
('MAX_MEMBER_EXE201', 8, NULL);

-- Thêm dữ liệu mẫu cho bảng Campus
INSERT INTO CAMPUS (campus_code, campus_name, location)
VALUES 
('FPT-HN', 'FPT Ha Noi', 'Ha Noi'),
('FPT-HCM', 'FPT Ho Chi Minh', 'Ho Chi Minh'),
('FPT-DN', 'FPT Da Nang', 'Da Nang');

-- Thêm dữ liệu mẫu cho bảng SUBJECT
INSERT INTO SUBJECT (subject_code, subject_name)
VALUES 
('EXE101', 'Entrepreneurship Basics'),
('EXE201', 'Advanced Entrepreneurship');

-- Thêm dữ liệu mẫu cho bảng Role
INSERT INTO ROLE (name, description)
VALUES 
('Admin', 'Administrator with full access'),
('Manager', 'Manager with project management privileges'),
('Student', 'Student participating in projects'),
('Mentor', 'Mentor providing guidance to projects');

-- Thêm dữ liệu mẫu cho bảng feature
INSERT INTO FEATURE (name, description)
VALUES 
('User Management', 'Feature to manage (view, create, update, delete, import) users'),
('Workshop Management', 'Feature to manage (view, create, update, delete) workshops'),
('Meeting Schedule Management', 'Feature to manage (view, create, update, delete) meeting schedules in the group'),
('Timeline Management', 'Feature to manage (view, update) timeline'),
('Syllabus', 'Feature to manage (view) syllabus session');

-- Thêm dữ liệu mẫu cho bảng Permission
INSERT INTO PERMISSION (feature_id, name, description)
VALUES 
(1, 'view_user', 'Permission to view users'),
(1, 'create_user', 'Permission to create users'),
(1, 'edit_user', 'Permission to edit users'),
(1, 'delete_user', 'Permission to delete users'),
(1, 'import_user', 'Permission to import users from ecel file'),
(2, 'view_workshop', 'Permission to view workshops'),
(2, 'create_workshop', 'Permission to create workshops'),
(2, 'edit_workshop', 'Permission to edit workshops'),
(2, 'delete_workshop', 'Permission to delete workshops'),
(3, 'view_meeting_schedule', 'Permission to view meeting schedules'),
(3, 'create_meeting_schedule', 'Permission to create meeting schedules'),
(3, 'edit_meeting_schedule', 'Permission to edit meeting schedules'),
(3, 'delete_meeting_schedule', 'Permission to delete meeting schedules'),
(4, 'view_timeline', 'Permission to view timeline'),
(4, 'edit_timeline', 'Permission to edit timeline'),
(5, 'view_syllabus', 'Permission to edit syllabus session');

-- Thêm dữ liệu mẫu cho bảng User
-- Default password: Uniexetask123456
INSERT INTO [USER] (full_name, [password], email, avatar, phone, campus_id, role_id)
VALUES 
('Admin User', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'uniexetask.it@gmail.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000001', 1, 1),
('Manager User', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'uniexetask.manager@gmail.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000002', 2, 2),
('Mentor User 1', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'uniexetask.mentor1@gmail.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000012', 1, 4),
('Mentor User 2', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor2@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000013', 2, 4),
('Mentor User 3', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor3@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000014', 2, 4),
('Mentor User 4', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor4@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000015', 3, 4),
('Mentor User 5', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor5@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000016', 3, 4),
(N'Nguyễn Huỳnh Đức Trí', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student1uniexetask@gmail.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0867892130', 2, 3),
(N'Phan Song Thảo', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'thaopsse162032@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0837250452', 2, 3),
(N'Lê Hòa Bình', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'binhlhse162087@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0913926749', 2, 3),
(N'Trần Hồng Hưng', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'hungthse162056@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0374312384', 2, 3);

-- Thêm dữ liệu mẫu cho bảng MENTOR
INSERT INTO MENTOR (user_id, specialty)
VALUES 
(3, 'Renewable Energy'),
(4, 'Urban Planning'),
(5, 'Data Science'),
(6, 'Artificial Intelligence'),
(7, 'Blockchain Technology');

-- Thêm dữ liệu mẫu cho bảng STUDENT
INSERT INTO STUDENT (user_id, lecturer_id, student_code, major, subject_id, isCurrentPeriod)
VALUES 
(8, 2,'SE162014', 'Software Engineering', 1, 1),
(9, 2,'SE162032', 'Software Engineering', 1, 1),
(10, 3,'SE162087', 'Software Engineering', 1, 1),
(11, 3,'SE162056', 'Software Engineering', 1, 1);

-- Thêm dữ liệu mẫu cho bảng GROUP
INSERT INTO [GROUP] (group_name, subject_id, hasMentor, status, isCurrentPeriod)
VALUES 
('UniExETask', 1, 1, 'Approved', 1);

-- Thêm dữ liệu mẫu cho bảng GROUP_MEMBER
INSERT INTO GROUP_MEMBER (group_id, student_id, role)
VALUES 
(1, 1, 'Leader'), 
(1, 2, 'Member'),
(1, 3, 'Member'), 
(1, 4, 'Member');

-- Thêm dữ liệu mẫu cho bảng PROJECT_MENTOR
INSERT INTO MENTOR_GROUP (group_id, mentor_id)
VALUES 
(1, 1);

-- Thêm dữ liệu mẫu cho bảng ROLE_PERMISSION
INSERT INTO ROLE_PERMISSION (role_id, permission_id)
VALUES 
(2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 14), (2, 15), (2, 6), (2, 7), (2, 8), (2, 9),
(3, 10), (3, 6), (3, 14), (3, 16),
(4, 6), (4, 10), (4, 11), (4, 12), (4, 13), (4, 14), (4, 16);

-- Thêm dữ liệu mẫu cho bảng CHAT_GROUP
INSERT INTO CHAT_GROUP (chat_group_name, chat_group_avatar, created_by, owner_id, group_id, type)
VALUES 
('UniExETask', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', 8, 8, 1, 'Group');

-- Thêm dữ liệu mẫu cho bảng CHAT_GROUP
INSERT INTO USER_CHAT_GROUP (user_id, chat_group_id)
VALUES 
(8, 1),
(9, 1),
(10, 1),
(11, 1),
(3, 1);

-- Thêm dữ liệu mẫu cho bảng CHAT_MESSAGE
INSERT INTO CHAT_MESSAGE (chat_group_id, user_id, message_content)
VALUES 
(1, 8, 'Welcome to the UniExETask Group!');

-- Thêm dữ liệu mẫu cho bảng TOPIC
INSERT INTO TOPIC (topic_code, topic_name, description)
VALUES 
('TP000', 'Uni EXE Task', 'Platform to manage and support the Experiential Entrepreneurship subject of FPT University.');

-- Thêm dữ liệu mẫu cho bảng PROJECT
INSERT INTO PROJECT (group_id, topic_id, start_date, end_date, subject_id, status, isCurrentPeriod)
VALUES 
(1, 1, '2024-09-01', '2025-01-01', 1, 'In_Progress', 1);

-- Thêm dữ liệu mẫu cho bảng TASK
INSERT INTO TASK (project_id, task_name, description, start_date, end_date, status)
VALUES 
(1, 'Research Phase', 'Complete the research phase of the project', '2024-09-03', '2024-09-17', 'Completed'),
(1, 'Project Initiation', 'Define project scope and objectives', '2024-09-15', '2024-09-28', 'Completed'),
(1, 'Requirements Gathering', 'Collect and document project requirements', '2024-09-25', '2024-10-15', 'Overdue'),
(1, 'System Design', 'Create system architecture and design documents', '2024-11-10', '2024-11-30', 'In_Progress'),
(1, 'Database Design', 'Design database schema and relationships', '2024-11-15', '2024-12-5', 'In_Progress'),
(1, 'UI/UX Design', 'Create user interface mockups and prototypes', '2024-11-17', '2024-12-10', 'In_Progress'),
(1, 'Backend Development', 'Implement server-side logic and APIs', '2024-12-12', '2024-12-23', 'Not_Started'),
(1, 'Frontend Development', 'Implement client-side user interface', '2024-12-14', '2024-12-28', 'Not_Started');

-- Thêm dữ liệu mẫu cho bảng PROJECT_PROGRESS
INSERT INTO PROJECT_PROGRESS (project_id, progress_percentage, updated_date, note, isDeleted)
VALUES 
(1, 20.00, '2024-09-15', 'Research phase started', 1),
(1, 35.00, '2024-10-15', 'Requirements collected and documented', 1),
(1, 45.00, '2024-10-23', 'System design initiated', 0);

-- Thêm dữ liệu mẫu cho bảng TASK_PROGRESS
INSERT INTO TASK_PROGRESS (task_id, progress_percentage, updated_date, note)
VALUES 
(1, 100.00, '2024-09-15', 'Finished'),
(2, 100.00, '2024-09-15', 'Project initiation completed successfully'),
(3, 0.00, '2024-09-01', 'Not started yet'),
(4, 80.00, '2024-10-23', 'System design halfway completed, facing some delays'),
(5, 40.00, '2024-11-01', 'Initial database schema designed'),
(6, 0.00, '2024-09-01', 'UI mockups under review'),
(7, 0.00, '2024-09-01', 'Not started yet'),
(8, 0.00, '2024-09-01', 'Not started yet');

-- Thêm dữ liệu mẫu cho bảng TASK
INSERT INTO TASK_ASSIGN(task_id, student_id, assigned_date)
VALUES 
(1, 1, '2024-09-01'),
(1, 2, '2024-09-01'),
(2, 2, '2024-09-01'),
(3, 1, '2024-09-01'),
(3, 3, '2024-09-01'),
(3, 4, '2024-09-01'),
(4, 1, '2024-09-01'),
(4, 2, '2024-09-01'),
(5, 3, '2024-09-01'),
(5, 4, '2024-09-01'),
(6, 3, '2024-09-01'),
(6, 2, '2024-09-01'),
(6, 1, '2024-09-01'),
(7, 4, '2024-09-01'),
(8, 2, '2024-09-01'),
(8, 1, '2024-09-01');

-- Thêm dữ liệu mẫu cho bảng TASK_DETAIL
INSERT INTO TASK_DETAIL (task_id, task_detail_name, progress_percentage, isCompleted, isDeleted)
VALUES 
-- Task 1: 'Research Phase'
(1, N'Nghiên cứu tài liệu và công nghệ liên quan', 80.00, 1, 0),
(1, N'Phân tích yêu cầu về nghiên cứu', 20.00, 1, 0),

-- Task 2: 'Project Initiation'
(2, N'Xác định phạm vi dự án', 40.00, 1, 0),
(2, N'Phân bổ nguồn lực và lập kế hoạch', 30.00, 1, 0),
(2, N'Báo cáo', 30.00, 1, 0),

-- Task 3: 'Requirements Gathering'
(3, N'Thu thập yêu cầu từ các bên liên quan', 60.00, 0, 0),
(3, N'Phân tích và tài liệu hóa yêu cầu', 40.00, 0, 0),

-- Task 4: 'System Design'
(4, N'Thiết kế giao diện người dùng (UI)', 45.00, 1, 0),
(4, N'Thiết kế kiến trúc hệ thống', 35.00, 1, 0),
(4, N'Báo cáo 4', 20.00, 0, 0),

-- Task 5: 'Database Design'
(5, N'Thiết kế cơ sở dữ liệu ban đầu', 40.00, 1, 0),
(5, N'Tối ưu hóa cơ sở dữ liệu', 60.00, 0, 0),

-- Task 6: 'UI/UX Design'
(6, N'Tạo mockups cho trang chủ', 30.00, 0, 0),
(6, N'Review và phản hồi từ đội ngũ thiết kế', 70.00, 0, 0),

-- Task 7: 'Backend Development'
(7, N'Phát triển API cho người dùng', 55.00, 0, 0),
(7, N'Phát triển API cho quản lý dự án', 45.00, 0, 0),

-- Task 8: 'Frontend Development'
(8, N'Phát triển giao diện cho đăng nhập', 38.00, 0, 0),
(8, N'Phát triển giao diện cho quản lý dự án', 62.00, 0, 0);

-- Thêm dữ liệu mẫu cho bảng NOTIFICATION
INSERT INTO NOTIFICATION (sender_id, receiver_id, message, type, created_at, status)
VALUES 
(8, 9, 'You received an invitation to join the UniExETask group.', 'Group_Request', GETDATE(), 'Read'),
(8, 10, 'You received an invitation to join the UniExETask group.', 'Group_Request', GETDATE(), 'Read'),
(8, 11, 'You received an invitation to join the UniExETask group.', 'Group_Request', GETDATE(), 'Read');

-- Thêm dữ liệu mẫu cho bảng GROUP_INVITE
INSERT INTO GROUP_INVITE (group_id, notification_id, inviter_id, invitee_id, created_date, updated_date, status)
VALUES 
(1, 1, 8, 9, GETDATE(), GETDATE(), 'Accepted'),
(1, 2, 8, 10, GETDATE(), GETDATE(), 'Accepted'),
(1, 3, 8, 11, GETDATE(), GETDATE(), 'Accepted');

-- Thêm dữ liệu mẫu cho bảng WORKSHOP
INSERT INTO WORKSHOP (name, description, start_date, end_date, location, reg_url, status)
VALUES 
('Innovation Workshop', 'Workshop on innovation and entrepreneurship', '2024-10-10', '2024-10-12', N'FPT Hà Nội', 'http://example.com/register', 'Completed'),
('Tech Expo', 'Exhibition on smart city technologies', '2024-12-01', '2024-12-03', N'FPT Hồ Chí Minh', 'http://example.com/register', 'Not_Started');

INSERT INTO REG_MEMBER_FORM (group_id, description, status)
VALUES 
    (1, N'Tôi cần tìm 1 FE có đủ kỹ năng để làm việc nhóm', 1);

INSERT INTO MILESTONE (milestone_name, description, percentage, subject_id, start_date, end_date, created_date, updated_date, isDeleted)
VALUES
-- Milestone 1: Project Initiation Phase
('Project Initiation', 'Complete an overview plan and define entrepreneurship objectives', 15, 1, '2024-01-01', '2024-01-15', GETDATE(), NULL, 0),

-- Milestone 2: Market Research
('Market Research', 'Analyze customer needs and conduct market surveys', 20, 1, '2024-01-16', '2024-01-30', GETDATE(), NULL, 0),

-- Milestone 3: Business Plan Development
('Business Plan Development', 'Complete a detailed business plan', 25, 1, '2024-02-01', '2024-02-15', GETDATE(), NULL, 0),

-- Milestone 4: Prototyping
('Prototyping', 'Design and test a prototype of the product/service', 20, 1, '2024-02-16', '2024-03-01', GETDATE(), NULL, 0),

-- Milestone 5: Evaluation and Refinement
('Evaluation and Refinement', 'Collect feedback and improve based on real-world testing', 15, 1, '2024-03-02', '2024-03-15', GETDATE(), NULL, 0),

-- Milestone 6: Final Presentation
('Final Presentation', 'Prepare and present the project outcomes to a panel', 5, 1, '2024-03-16', '2024-03-20', GETDATE(), NULL, 0);
INSERT INTO MILESTONE (milestone_name, description, percentage, subject_id, start_date, end_date, created_date, updated_date, isDeleted)
VALUES
-- Milestone 1: Advanced Market Analysis
('Advanced Market Analysis', 'Conduct in-depth market research with segmentation and competitive analysis', 15, 2, '2024-04-01', '2024-04-15', GETDATE(), NULL, 0),

-- Milestone 2: Financial Planning and Funding Strategy
('Financial Planning & Funding Strategy', 'Develop financial projections and identify funding opportunities', 20, 2, '2024-04-16', '2024-04-30', GETDATE(), NULL, 0),

-- Milestone 3: Building a Scalable Model
('Building a Scalable Model', 'Design scalable processes and identify key resources for growth', 25, 2, '2024-05-01', '2024-05-15', GETDATE(), NULL, 0),

-- Milestone 4: Product Refinement and User Feedback
('Product Refinement', 'Incorporate user feedback to improve product/service offerings', 15, 2, '2024-05-16', '2024-05-31', GETDATE(), NULL, 0),

-- Milestone 5: Legal and Compliance
('Legal and Compliance', 'Ensure the business adheres to legal standards and secures necessary certifications', 10, 2, '2024-06-01', '2024-06-10', GETDATE(), NULL, 0),

-- Milestone 6: Strategic Presentation for Investors
('Investor Pitch', 'Prepare and present a strategic pitch to potential investors', 15, 2, '2024-06-11', '2024-06-20', GETDATE(), NULL, 0);

INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Idea Validation', 'Evaluate the feasibility of the business idea based on market trends', 30, 1, GETDATE(), GETDATE(), 0),
('Market Research Report', 'Provide a detailed report on the target market', 40, 1, GETDATE(), GETDATE(), 0),
('SWOT Analysis', 'Complete a comprehensive SWOT analysis for the business', 30, 1, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Executive Summary', 'Summarize the core idea of the business in a concise manner', 25, 2, GETDATE(), GETDATE(), 0),
('Marketing Plan', 'Develop a strategic marketing plan', 35, 2, GETDATE(), GETDATE(), 0),
('Financial Projection', 'Create accurate financial projections for 1-3 years', 25, 2, GETDATE(), GETDATE(), 0),
('Risk Management Plan', 'Outline strategies to handle potential risks', 15, 2, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Role Assignment', 'Assign specific roles and responsibilities to team members', 30, 3, GETDATE(), GETDATE(), 0),
('Team Structure Design', 'Develop an effective team structure for the business', 30, 3, GETDATE(), GETDATE(), 0),
('Conflict Resolution Strategy', 'Create a strategy for managing team conflicts', 40, 3, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Prototype Design', 'Create a functional prototype of the product/service', 40, 4, GETDATE(), GETDATE(), 0),
('User Feedback', 'Collect and analyze feedback from initial users', 30, 4, GETDATE(), GETDATE(), 0),
('Iteration Plan', 'Plan for improving the prototype based on feedback', 30, 4, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Segment Analysis', 'Identify and analyze market segments', 30, 5, GETDATE(), GETDATE(), 0),
('Competitor Analysis', 'Evaluate competitors to identify unique selling points', 40, 5, GETDATE(), GETDATE(), 0),
('Trend Projection', 'Analyze market trends and their potential impact', 30, 5, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Presentation Quality', 'Deliver a well-structured and engaging presentation', 40, 6, GETDATE(), GETDATE(), 0),
('Business Model Explanation', 'Clearly articulate the business model and its scalability', 35, 6, GETDATE(), GETDATE(), 0),
('Investor Q&A', 'Respond effectively to investor questions and concerns', 25, 6, GETDATE(), GETDATE(), 0);

INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Current Term Duration EXE101', N'Current Term Duration EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Current Term Duration EXE201', N'Current Term Duration EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Group EXE101', N'Finalize Group EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-15T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Group EXE201', N'Finalize Group EXE201', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Mentor EXE101', N'Finalize Mentor EXE201', CAST(N'2024-11-28T00:00:00.000' AS DateTime), CAST(N'2024-11-28T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Mentor EXE201', N'Finalize Mentor EXE201', CAST(N'2024-11-28T00:00:00.000' AS DateTime), CAST(N'2024-11-28T00:00:00.000' AS DateTime), 2)