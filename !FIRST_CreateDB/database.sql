USE [master]
GO
/****** Object:  Database [teste]    Script Date: 12/04/2018 16:02:57 ******/
CREATE DATABASE [teste]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'teste', FILENAME = N'c:\db\teste.mdf' , SIZE = 8192KB , MAXSIZE = 20971520KB , FILEGROWTH = 10%)
 LOG ON 
( NAME = N'teste_log', FILENAME = N'c:\db\teste_log.ldf' , SIZE = 8192KB , MAXSIZE = 1048576KB , FILEGROWTH = 10%)
GO
ALTER DATABASE [teste] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [teste].[dbo].[sp_fulltext_database] @action = 'enable'
end 
GO
ALTER DATABASE [teste] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [teste] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [teste] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [teste] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [teste] SET ARITHABORT OFF 
GO
ALTER DATABASE [teste] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [teste] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [teste] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [teste] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [teste] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [teste] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [teste] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [teste] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [teste] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [teste] SET  DISABLE_BROKER 
GO
ALTER DATABASE [teste] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [teste] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [teste] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [teste] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [teste] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [teste] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [teste] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [teste] SET RECOVERY FULL 
GO
ALTER DATABASE [teste] SET  MULTI_USER 
GO
ALTER DATABASE [teste] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [teste] SET DB_CHAINING OFF 
GO
ALTER DATABASE [teste] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [teste] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [teste] SET DELAYED_DURABILITY = DISABLED 
GO
USE [teste]
GO
/****** Object:  UserDefinedTableType [dbo].[FORNECEDOR_REGIAO_TYPE]    Script Date: 07/11/2017 13:52:15 ******/
CREATE TYPE [dbo].[FORNECEDOR_REGIAO_TYPE] AS TABLE(
	[ID_FORNECEDOR] [bigint] NOT NULL,
	[ID_REGIAO] [bigint] NOT NULL
)
GO
/****** Object:  Table [dbo].[Estado]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estado](
	[IdEstado] [int] IDENTITY(1,1) NOT NULL,
	[Descricao] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED 
(
	[IdEstado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fornecedor]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fornecedor](
	[IdFornecedor] [bigint] IDENTITY(1,1) NOT NULL,
	[CNPJ] [nvarchar](20) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Fornecedor] PRIMARY KEY CLUSTERED 
(
	[IdFornecedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FornecedorRegiao]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FornecedorRegiao](
	[IdFornecedorRegiao] [bigint] IDENTITY(1,1) NOT NULL,
	[IdFornecedor] [bigint] NOT NULL,
	[IdRegiao] [bigint] NOT NULL,
 CONSTRAINT [PK_FornecedorRegiao] PRIMARY KEY CLUSTERED 
(
	[IdFornecedorRegiao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Regiao]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regiao](
	[IdRegiao] [bigint] IDENTITY(1,1) NOT NULL,
	[IdEstado] [int] NOT NULL,
	[Descricao] [nvarchar](50) NOT NULL,
	[Ativo] [bit] NOT NULL,
 CONSTRAINT [PK_Regiao] PRIMARY KEY CLUSTERED 
(
	[IdRegiao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Estado] ON 

INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (1, N'AC')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (2, N'AL')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (4, N'AM')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (3, N'AP')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (5, N'BA')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (6, N'CE')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (7, N'DF')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (8, N'ES')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (9, N'GO')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (10, N'MA')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (13, N'MG')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (12, N'MS')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (11, N'MT')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (14, N'PA')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (15, N'PB')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (17, N'PE')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (18, N'PI')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (16, N'PR')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (19, N'RJ')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (20, N'RN')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (22, N'RO')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (23, N'RR')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (21, N'RS')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (24, N'SC')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (26, N'SE')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (25, N'SP')
INSERT [dbo].[Estado] ([IdEstado], [Descricao]) VALUES (27, N'TO')
SET IDENTITY_INSERT [dbo].[Estado] OFF
SET IDENTITY_INSERT [dbo].[Fornecedor] ON 

INSERT [dbo].[Fornecedor] ([IdFornecedor], [CNPJ], [Nome]) VALUES (1, N'73.210.570/0001-80', N'IBM')
INSERT [dbo].[Fornecedor] ([IdFornecedor], [CNPJ], [Nome]) VALUES (4, N'98.993.415/0001-08', N'Microsoft')
INSERT [dbo].[Fornecedor] ([IdFornecedor], [CNPJ], [Nome]) VALUES (5, N'74.329.141/0001-99', N'Apple')
SET IDENTITY_INSERT [dbo].[Fornecedor] OFF
SET IDENTITY_INSERT [dbo].[FornecedorRegiao] ON 

INSERT [dbo].[FornecedorRegiao] ([IdFornecedorRegiao], [IdFornecedor], [IdRegiao]) VALUES (7, 1, 7)
INSERT [dbo].[FornecedorRegiao] ([IdFornecedorRegiao], [IdFornecedor], [IdRegiao]) VALUES (8, 4, 5)
INSERT [dbo].[FornecedorRegiao] ([IdFornecedorRegiao], [IdFornecedor], [IdRegiao]) VALUES (9, 4, 6)
INSERT [dbo].[FornecedorRegiao] ([IdFornecedorRegiao], [IdFornecedor], [IdRegiao]) VALUES (11, 5, 6)
INSERT [dbo].[FornecedorRegiao] ([IdFornecedorRegiao], [IdFornecedor], [IdRegiao]) VALUES (10, 5, 8)
SET IDENTITY_INSERT [dbo].[FornecedorRegiao] OFF
SET IDENTITY_INSERT [dbo].[Regiao] ON 

INSERT [dbo].[Regiao] ([IdRegiao], [IdEstado], [Descricao], [Ativo]) VALUES (5, 25, N'Grande Campinas', 1)
INSERT [dbo].[Regiao] ([IdRegiao], [IdEstado], [Descricao], [Ativo]) VALUES (6, 25, N'Grande São Paulo', 0)
INSERT [dbo].[Regiao] ([IdRegiao], [IdEstado], [Descricao], [Ativo]) VALUES (7, 11, N'Campo Grande', 1)
INSERT [dbo].[Regiao] ([IdRegiao], [IdEstado], [Descricao], [Ativo]) VALUES (8, 25, N'Grande ABC', 1)
INSERT [dbo].[Regiao] ([IdRegiao], [IdEstado], [Descricao], [Ativo]) VALUES (9, 19, N'Rio de Janeiro', 1)
SET IDENTITY_INSERT [dbo].[Regiao] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_Estado]    Script Date: 12/04/2018 16:02:57 ******/
ALTER TABLE [dbo].[Estado] ADD  CONSTRAINT [UQ_Estado] UNIQUE NONCLUSTERED 
(
	[Descricao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_Fornecedor]    Script Date: 12/04/2018 16:02:57 ******/
ALTER TABLE [dbo].[Fornecedor] ADD  CONSTRAINT [UQ_Fornecedor] UNIQUE NONCLUSTERED 
(
	[CNPJ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_FornecedorRegiao]    Script Date: 12/04/2018 16:02:57 ******/
ALTER TABLE [dbo].[FornecedorRegiao] ADD  CONSTRAINT [UQ_FornecedorRegiao] UNIQUE NONCLUSTERED 
(
	[IdFornecedor] ASC,
	[IdRegiao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FornecedorRegiao]  WITH CHECK ADD  CONSTRAINT [FK_FornecedorRegiao_Fornecedor] FOREIGN KEY([IdFornecedor])
REFERENCES [dbo].[Fornecedor] ([IdFornecedor])
GO
ALTER TABLE [dbo].[FornecedorRegiao] CHECK CONSTRAINT [FK_FornecedorRegiao_Fornecedor]
GO
ALTER TABLE [dbo].[FornecedorRegiao]  WITH CHECK ADD  CONSTRAINT [FK_FornecedorRegiao_Regiao] FOREIGN KEY([IdRegiao])
REFERENCES [dbo].[Regiao] ([IdRegiao])
GO
ALTER TABLE [dbo].[FornecedorRegiao] CHECK CONSTRAINT [FK_FornecedorRegiao_Regiao]
GO
ALTER TABLE [dbo].[Regiao]  WITH CHECK ADD  CONSTRAINT [FK_Regiao_Estado] FOREIGN KEY([IdEstado])
REFERENCES [dbo].[Estado] ([IdEstado])
GO
ALTER TABLE [dbo].[Regiao] CHECK CONSTRAINT [FK_Regiao_Estado]
GO
/****** Object:  StoredProcedure [dbo].[NEO_CONS_ESTADO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	CONSULTA TODOS OS ESTADOS BRASILEIROS
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_CONS_ESTADO](
	@ID INT
)
AS 
BEGIN TRY

	SELECT 
		E.IdEstado,
		E.Descricao 
	FROM Estado E (NOLOCK)
	WHERE ((@ID IS NOT NULL AND E.IdEstado = @ID) OR (@ID IS NULL))
	ORDER BY E.IdEstado

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_CONS_FORNECEDOR]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	CONSULTA OS FORNECEDORES
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_CONS_FORNECEDOR](
	@ID BIGINT	
)
AS 
BEGIN TRY

	SELECT 
		IdFornecedor,
		CNPJ,
		Nome
	FROM Fornecedor forn (NOLOCK)
	WHERE ((@ID IS NOT NULL AND forn.IdFornecedor = @ID) OR (@ID IS NULL))

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_CONS_FORNECEDOR_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	CONSULTA AS REGIÕES DO FORNECEDOR
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_CONS_FORNECEDOR_REGIAO](
	@ID BIGINT	
)
AS 
BEGIN TRY

	SELECT 
		r.IdRegiao,
		r.Descricao as descRegiao,
		r.Ativo,
		e.IdEstado,
		e.Descricao as descEstado
	FROM Fornecedor forn (NOLOCK)
	INNER JOIN FornecedorRegiao fr (NOLOCK)
	ON fr.IdFornecedor = forn.IdFornecedor
	INNER JOIN Regiao r (NOLOCK)
	ON r.IdRegiao = fr.IdRegiao
	INNER JOIN Estado e (NOLOCK)
	ON e.IdEstado = r.IdEstado
	WHERE ((@ID IS NOT NULL AND forn.IdFornecedor = @ID) OR (@ID IS NULL))

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_CONS_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	CONSULTA A TABELA REGIAO
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_CONS_REGIAO](
	@ID BIGINT	
)
AS 
BEGIN TRY

	SELECT 
		R.IDREGIAO,
		R.DESCRICAO,
		R.ATIVO,
		E.IDESTADO,
		E.DESCRICAO AS DESCRICAOESTADO
	FROM REGIAO R (NOLOCK)
	INNER JOIN ESTADO E (NOLOCK)
	ON E.IDESTADO = R.IDESTADO
	WHERE ((@ID IS NOT NULL AND R.IDREGIAO = @ID) OR (@ID IS NULL))
	ORDER BY R.DESCRICAO

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_INS_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	INSERE NA TABELA REGIAO
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_INS_REGIAO](
	@ID_UF INT,
	@REGIAO VARCHAR(50)
)
AS 
BEGIN TRY

	DECLARE @EXISTE INT
	DECLARE @MENSAGEM VARCHAR(100) 

	SET @EXISTE = (
					SELECT COUNT(1) 
					FROM Regiao (NOLOCK) 
					WHERE IdEstado = @ID_UF 
					AND Descricao = @REGIAO
				  )

	IF @EXISTE = 0
	BEGIN

		INSERT INTO Regiao
		(
			 IdEstado
			,Descricao
			,Ativo
		)
		VALUES
		(
			 @ID_UF
			,@REGIAO
			,0 
		)

	END
	ELSE
	BEGIN

	SET @MENSAGEM = 'Já existe essa região cadastrada'
	SELECT @MENSAGEM

	END


END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_ATUALIZA_ATIVA_INATIVA_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	ATIVA OU INATIVA A REGIÃO
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_ATUALIZA_ATIVA_INATIVA_REGIAO](
	@ID BIGINT,
	@ATIVO BIT
)
AS 
BEGIN TRY

	UPDATE Regiao
	SET Ativo = @ATIVO
	WHERE IdRegiao = @ID

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_ATUALIZA_FORNECEDOR_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	CONSULTA AS REGIÕES DOS FORNECEDORES
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_ATUALIZA_FORNECEDOR_REGIAO](
	@ID_FORNECEDOR BIGINT,	
	@FORN_REG FORNECEDOR_REGIAO_TYPE READONLY
)
AS 
BEGIN TRY
	-- DELETA AS REGIÕES
	DELETE FROM FornecedorRegiao
	WHERE IdFornecedor = @ID_FORNECEDOR

	--INSERE AS REGIÕES
	INSERT INTO FornecedorRegiao(IdRegiao, IdFornecedor)
	SELECT * FROM @FORN_REG

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
/****** Object:  StoredProcedure [dbo].[NEO_ATUALIZA_REGIAO]    Script Date: 12/04/2018 16:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:	Pedro Mendonca
-- Create date: 2018-04-12
-- Description:	ATUALIZA A REGIAO
-- ===============================================*/
CREATE PROCEDURE [dbo].[NEO_ATUALIZA_REGIAO](
	@ID BIGINT,
	@ID_ESTADO INT,
	@DESCRICAO_REGIAO VARCHAR(50)
)
AS 
BEGIN TRY

	DECLARE @EXISTE INT
	DECLARE @MENSAGEM VARCHAR(100) 

	SET @EXISTE = (
					SELECT COUNT(1) 
					FROM Regiao (NOLOCK)
					WHERE IdEstado = @ID_ESTADO 
					AND Descricao = @DESCRICAO_REGIAO
				  )

	IF(@EXISTE = 0)
	BEGIN

	UPDATE Regiao
	SET IdEstado = @ID_ESTADO,
	Descricao = @DESCRICAO_REGIAO
	WHERE IdRegiao = @ID

	END
	ELSE
	BEGIN

	SET @MENSAGEM = 'Esta regiao já existe.'
	SELECT @MENSAGEM

	END

END TRY

BEGIN CATCH
	DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
	SELECT @ErrMsg = ERROR_MESSAGE(),
           @ErrSeverity = ERROR_SEVERITY()

	RAISERROR(@ErrMsg, @ErrSeverity, 1)
END CATCH 
GO
USE [master]
GO
ALTER DATABASE [teste] SET  READ_WRITE 
GO
