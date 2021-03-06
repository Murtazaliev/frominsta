USE [INSTAGRAMAPP]
GO
/****** Object:  Table [dbo].[APPUSER]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[APPUSER](
	[USER_ID] [int] IDENTITY(1,1) NOT NULL,
	[USER_LOGIN] [varchar](50) NOT NULL,
	[USER_PASSWORD] [varchar](50) NOT NULL,
	[USER_ROLE_ID] [int] NOT NULL CONSTRAINT [DF_APPUSER_USER_ROLE_ID]  DEFAULT ((3)),
	[USER_LASTNAME] [varchar](50) NULL,
	[USER_FIRSTNAME] [varchar](50) NULL,
	[USER_PATR] [varchar](50) NULL,
	[USER_EMAIL] [varchar](50) NULL,
	[USER_PHONE] [varchar](max) NULL,
	[USER_MAX_TAG_COUNT] [int] NOT NULL CONSTRAINT [DF_APPUSER_USER_MAX_TAG]  DEFAULT ((3)),
	[USER_SLIDE_ROTATION] [int] NOT NULL CONSTRAINT [DF_APPUSER_USER_SLIDE_ROTATION]  DEFAULT ((5)),
	[USER_SLIDE_BATCH_SIZE] [int] NOT NULL CONSTRAINT [DF_APPUSER_USER_SLIDE_BATCH]  DEFAULT ((30)),
	[USER_BACKGROUND_IMG_URL] [varchar](max) NOT NULL CONSTRAINT [DF_APPUSER_USER_BACKGROUND_IMG_URL]  DEFAULT ('~/Content/img/show_bg.jpg'),
 CONSTRAINT [PK_USER] PRIMARY KEY CLUSTERED 
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EVENT]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EVENT](
	[EVENT_ID] [int] IDENTITY(1,1) NOT NULL,
	[EVENT_NAME] [varchar](max) NOT NULL,
 CONSTRAINT [PK_EVENT] PRIMARY KEY CLUSTERED 
(
	[EVENT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HASHTAG]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HASHTAG](
	[TAG_ID] [int] IDENTITY(1,1) NOT NULL,
	[USER_ID] [int] NOT NULL,
	[TAG_CAPTION] [varchar](max) NOT NULL,
 CONSTRAINT [PK_HASHTAG] PRIMARY KEY CLUSTERED 
(
	[TAG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LOG]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LOG](
	[LOG_ID] [int] IDENTITY(1,1) NOT NULL,
	[EVENT_ID] [int] NOT NULL,
	[LOG_TIME] [datetime] NOT NULL CONSTRAINT [DF_LOG_LOG_TIME]  DEFAULT (getdate()),
	[USER_ID] [int] NOT NULL,
	[LOG_DESCRIPTION] [varchar](max) NULL,
 CONSTRAINT [PK_LOG] PRIMARY KEY CLUSTERED 
(
	[LOG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MEDIATAG]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MEDIATAG](
	[MEDIA_ID] [int] IDENTITY(1,1) NOT NULL,
	[USER_ID] [int] NOT NULL,
	[ORDER_ID] [int] NOT NULL,
	[TAG_CAPTION] [varchar](max) NOT NULL,
	[INSTAGRAM_MEDIA_ID] [varchar](max) NULL,
	[INSTAGRAM_MEDIA_LOW_RES_URL] [varchar](max) NULL,
	[INSTAGRAM_MEDIA_STANDARD_RES_URL] [varchar](max) NULL,
	[INSTAGRAM_MEDIA_THUMBNAIL_URL] [varchar](max) NULL,
	[INSTAGRAM_USER_ID] [varchar](max) NULL,
	[INSTAGRAM_USER_NAME] [varchar](max) NULL,
	[INSTAGRAM_USER_PROFILEPICTURE] [varchar](max) NULL,
	[INSTAGRAM_CAPTION] [varchar](max) NULL,
	[BAN] [bit] NOT NULL CONSTRAINT [DF_IMAGE_BAN]  DEFAULT ((0)),
	[DELETED] [bit] NOT NULL CONSTRAINT [DF_MEDIATAG_DELETED]  DEFAULT ((0)),
 CONSTRAINT [PK_IMAGE] PRIMARY KEY CLUSTERED 
(
	[MEDIA_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ROLE]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ROLE](
	[ROLE_ID] [int] IDENTITY(1,1) NOT NULL,
	[ROLE_NAME] [varchar](50) NOT NULL,
	[ROLE_DISCR] [varchar](max) NULL,
 CONSTRAINT [PK_ROLE] PRIMARY KEY CLUSTERED 
(
	[ROLE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SHOW]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SHOW](
	[SHOW_ID] [int] IDENTITY(1,1) NOT NULL,
	[SHOW_START] [datetime] NOT NULL CONSTRAINT [DF_ORDER_ORDER_START]  DEFAULT (getdate()),
	[SHOW_END] [datetime] NOT NULL CONSTRAINT [DF_ORDER_ORDER_END]  DEFAULT (getdate()),
	[USER_ID] [int] NOT NULL,
	[USER_LOGIN] [varchar](50) NULL,
	[PAID] [bit] NOT NULL CONSTRAINT [DF_ORDER_PAID]  DEFAULT ((0)),
	[ALLOWMOD] [bit] NOT NULL CONSTRAINT [DF_ORDER_ALLOWMOD]  DEFAULT ((0)),
 CONSTRAINT [PK_ORDER] PRIMARY KEY CLUSTERED 
(
	[SHOW_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VARIABLE]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VARIABLE](
	[VAR_NAME] [varchar](100) NOT NULL,
	[VAR_VALUE] [varchar](255) NOT NULL,
	[VAR_DISCR] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HASHTAG]  WITH CHECK ADD  CONSTRAINT [FK_HASHTAG_APPUSER] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[APPUSER] ([USER_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HASHTAG] CHECK CONSTRAINT [FK_HASHTAG_APPUSER]
GO
ALTER TABLE [dbo].[MEDIATAG]  WITH CHECK ADD  CONSTRAINT [FK_MEDIATAG_APPUSER] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[APPUSER] ([USER_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MEDIATAG] CHECK CONSTRAINT [FK_MEDIATAG_APPUSER]
GO
ALTER TABLE [dbo].[SHOW]  WITH CHECK ADD  CONSTRAINT [FK_SHOW_APPUSER] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[APPUSER] ([USER_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SHOW] CHECK CONSTRAINT [FK_SHOW_APPUSER]
GO
/****** Object:  StoredProcedure [dbo].[CanUserModerateShow]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[CanUserModerateShow]
(
@user_id int,
@res bit OUTPUT
)
AS
BEGIN
	SET @res=0
	
	IF Exists(SELECT SHOW_ID FROM SHOW WHERE GetDate() BETWEEN SHOW_START AND SHOW_END AND PAID=1 AND ALLOWMOD=1)
	   SET @res=1
END
GO
/****** Object:  StoredProcedure [dbo].[CanUserTranslateShow]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[CanUserTranslateShow]
(
@user_id int,
@res bit OUTPUT
)
AS
BEGIN
	SET @res=0
	
	IF Exists(SELECT SHOW_ID FROM SHOW WHERE GetDate() BETWEEN SHOW_START AND SHOW_END AND PAID=1 AND [USER_ID]=@user_id)
	   SET @res=1
END
GO
/****** Object:  StoredProcedure [dbo].[ChangeUserPassword]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ChangeUserPassword]
(
@user_login varchar(50),
@oldpassword varchar(100),
@newpassword varchar(100),
@res bit OUTPUT
)
AS
BEGIN
	SET @res=0
    UPDATE APPUSER SET USER_PASSWORD=@newpassword WHERE lower(USER_LOGIN)=lower(@user_login)  AND USER_PASSWORD=@oldpassword
	if @@ROWCOUNT>0
		SET @res=1
END
GO
/****** Object:  StoredProcedure [dbo].[HashTagDelete]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[HashTagDelete]
(
@tag_caption varchar(max)
)
AS
BEGIN
	DELETE HASHTAG WHERE LOWER(TAG_CAPTION)=LOWER(@tag_caption)
	DELETE MEDIATAG WHERE LOWER(TAG_CAPTION)=LOWER(@tag_caption)
END
GO
/****** Object:  StoredProcedure [dbo].[HashTagExists]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[HashTagExists]
(
@tag_caption varchar(max),
@res bit OUTPUT
)
AS
BEGIN
	SET @res=0
	IF Exists(SELECT TAG_ID FROM HASHTAG WHERE LOWER(TAG_CAPTION)=LOWER(@tag_caption))
		SET @res=1
END
GO
/****** Object:  StoredProcedure [dbo].[HashTagInsert]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[HashTagInsert]
(
@user_id int,
@tag_caption varchar(max)
)
AS
BEGIN
	INSERT HASHTAG([USER_ID], TAG_CAPTION)
	VALUES (@user_id, @tag_caption)
END
GO
/****** Object:  StoredProcedure [dbo].[LogAddEvent]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[LogAddEvent]
(
@user_id int,
@event_id int,
@descr varchar(max)
)
AS
BEGIN
	INSERT [LOG]([USER_ID], EVENT_ID, LOG_DESCRIPTION)
	VALUES (@user_id, @event_id, @descr)
END
GO
/****** Object:  StoredProcedure [dbo].[RegisterNewUser]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[RegisterNewUser]
(
@user_login varchar(50),
@user_email varchar(50),
@user_lastname varchar(50),
@user_firstname varchar(50),
@user_patr varchar(50),
@user_phone varchar(500),
@user_password varchar(50),
@role_id int,
@user_id int OUTPUT
)
AS
BEGIN
	INSERT APPUSER(USER_LOGIN, USER_EMAIL, USER_LASTNAME, USER_FIRSTNAME, USER_PATR, USER_PHONE, USER_PASSWORD, USER_ROLE_ID)
	VALUES (@user_login, @user_email, @user_lastname, @user_firstname, @user_patr, @user_phone, @user_password, @role_id)
	
	SET @user_id=@@IDENTITY
	
	/* добавить хештэг по умолчанию */
	INSERT HASHTAG ([USER_ID], TAG_CAPTION) VALUES(@user_id, @user_login + '_frominsta')
END
GO
/****** Object:  StoredProcedure [dbo].[ShowInsert]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ShowInsert]
(
@user_id int,
@show_strat datetime,
@show_end datetime,
@paid bit,
@allowmod bit,
@show_id int OUTPUT
)
AS
BEGIN
	INSERT SHOW([USER_ID], SHOW_START, SHOW_END, PAID, ALLOWMOD)
	VALUES(@user_id, @show_strat, @show_end, @paid, @allowmod)
	
	SET @show_id=@@IDENTITY
END
GO
/****** Object:  StoredProcedure [dbo].[ShowSave]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ShowSave]
(
@show_id int OUTPUT,
@user_id int,
@show_start datetime,
@show_end datetime,
@paid bit,
@allowmod bit
)
AS
BEGIN
	IF @show_id>0
	   exec ShowUpdate @show_id, @user_id, @show_start, @show_end, @paid, @allowmod
	ELSE
	   exec ShowInsert @user_id, @show_start, @show_end, @paid, @allowmod, @show_id OUTPUT
END
GO
/****** Object:  StoredProcedure [dbo].[ShowUpdate]    Script Date: 01.06.2018 20:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[ShowUpdate]
(
@show_id int,
@user_id int,
@show_strat datetime,
@show_end datetime,
@paid bit,
@allowmod bit
)
AS
BEGIN
	UPDATE SHOW
	SET [USER_ID] = @user_id, 
	SHOW_START = @show_strat,
	SHOW_END = @show_end,
	PAID = @paid,
	ALLOWMOD = @allowmod
	WHERE SHOW_ID=@show_id
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Макс количество тегов пользователя' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'APPUSER', @level2type=N'COLUMN',@level2name=N'USER_MAX_TAG_COUNT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Период смены слайдов в сек (5-20)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'APPUSER', @level2type=N'COLUMN',@level2name=N'USER_SLIDE_ROTATION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Количество слайдов, загружаемых при одном запросе (20-50)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'APPUSER', @level2type=N'COLUMN',@level2name=N'USER_SLIDE_BATCH_SIZE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Фоновый рисунок' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'APPUSER', @level2type=N'COLUMN',@level2name=N'USER_BACKGROUND_IMG_URL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID пользователя' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HASHTAG', @level2type=N'COLUMN',@level2name=N'USER_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID закза' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MEDIATAG', @level2type=N'COLUMN',@level2name=N'ORDER_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пометка на удаление' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MEDIATAG', @level2type=N'COLUMN',@level2name=N'DELETED'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Начало действия заказа' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'SHOW_START'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Конец действия заказа' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'SHOW_END'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID пользователя' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'USER_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Логин для построения удобочитаемых роутов' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'USER_LOGIN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Оплачено' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'PAID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Разрешить модерирование' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SHOW', @level2type=N'COLUMN',@level2name=N'ALLOWMOD'
GO
