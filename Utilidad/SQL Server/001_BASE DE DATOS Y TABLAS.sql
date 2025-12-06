go
use master
go
IF NOT EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE NAME = 'DBVENTAS_WEB')
CREATE DATABASE DBVENTAS_WEB

GO 

USE DBVENTAS_WEB

GO

--(1) TABLA ROL
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ROL')
create table ROL(
	RolID int primary key identity(1,1),
	Descripcion varchar(60),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

GO

--(2) TABLA TIENDA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TIENDA')
create table TIENDA(
	SucursalID int primary key identity(1,1),
	Nombre varchar(60),
	RFC varchar(60),
	Direccion varchar(100),
	Telefono varchar(50),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

GO

--(3) TABLA MENU
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'MENU')
create table MENU(
	MenuID int primary key identity(1,1),
	Nombre varchar(60),
	Icono varchar(60),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

GO

--(4) TABLA SUBMENU
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SUBMENU')
create table SUBMENU(
	SubMenuID int primary key identity(1,1),
	MenuID int references MENU(MenuID),
	Nombre varchar(60),
	Controlador varchar(60),
	Vista varchar(50),
	Icono varchar(50),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

GO

--(5) TABLA USUARIO
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'USUARIO')
create table USUARIO(
	UsuarioID int primary key identity(1,1),
	Nombres varchar(60),
	Apellidos varchar(60),
	Correo varchar(60),
	Clave varchar(100),
	SucursalID int references TIENDA(SucursalID),
	RolID int references ROL(RolID),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

GO

--(6) TABLA PERMISOS
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PERMISOS')
create table PERMISOS(
	PermisosID int primary key identity(1,1),
	RolID int references ROL(RolID),
	SubMenuID int references SUBMENU(SubMenuID),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go
--(7) TABLA PROVEEDOR
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PROVEEDOR')
create table PROVEEDOR(
	ProveedorID int primary key identity(1,1),
	RFC varchar(50),
	RazonSocial varchar(100),
	Telefono varchar(50),
	Correo varchar(50),
	Direccion varchar(50),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go

--(8) TABLA CATEGORIA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CATEGORIA')
create table CATEGORIA(
	CategoriaID int primary key identity(1,1),
	Descripcion varchar(100),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go


--(8) TABLA PRODUCTO
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PRODUCTO')
create table PRODUCTO(
	ProductoID int primary key identity(1,1),
	Codigo varchar(100),
	ValorCodigo int,
	Nombre varchar(100),
	Descripcion varchar(100),
	CategoriaID int references CATEGORIA(CategoriaID),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go


--(8) TABLA PRODUCTO_TIENDA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PRODUCTO_TIENDA')
create table PRODUCTO_TIENDA(
	ProductoSucursalID int primary key identity(1,1),
	ProductoID int references PRODUCTO(ProductoID),
	SucursalID int references TIENDA(SucursalID),
	PrecioUnidadCompra decimal(18,2) default 0,
	PrecioUnidadVenta decimal(18,2) default 0,
	Stock bigint default 0,
	Activo bit default 1,
	Iniciado bit default 0,
	FechaRegistro datetime default getdate()
)

go

--(9) TABLA COMPRA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'COMPRA')
create table COMPRA(
	CompraID int primary key identity(1,1),
	UsuarioID int references USUARIO(UsuarioID),
	ProveedorID int references PROVEEDOR(ProveedorID),
	SucursalID int references TIENDA(SucursalID),
	TotalCosto decimal(18,2) default 0,
	TipoComprobante varchar(50) default 'Boleta',
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go

--(10) TABLA DETALLE_COMPRA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'DETALLE_COMPRA')
create table DETALLE_COMPRA(
	DetalleCompraID int primary key identity(1,1),
	CompraID int references COMPRA(CompraID),
	ProductoID int references Producto(ProductoID),
	Cantidad int,
	PrecioUnitarioCompra decimal(18,2),
	PrecioUnitarioVenta decimal(18,2),
	TotalCosto decimal(18,2),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go
-- (10) TABLA CLIENTE
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME = 'CLIENTE'
)
BEGIN
CREATE TABLE CLIENTE(
    ClienteID INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(200),
    RFC VARCHAR(20),
    Email VARCHAR(100),
    Telefono VARCHAR(50),
    Direccion VARCHAR(200),
    Ciudad VARCHAR(100),
    Estado VARCHAR(100),
    CodigoPostal VARCHAR(20),

    LimiteCredito DECIMAL(18,2) NULL,
    SaldoPendiente DECIMAL(18,2) NULL,

    Activo BIT DEFAULT 1,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Notas VARCHAR(MAX),

    TipoFinanciamiento VARCHAR(20),
    DiasCredito INT NULL,
    MontoCredito DECIMAL(18,2) NULL,
    UnidadesCredito INT NULL,

    TipoPrecio VARCHAR(20)
)
END


go 

-- (11) TABLA VENTA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'VENTA')
create table VENTA(
	VentaID int primary key identity(1,1),
	Codigo varchar(100),
	ValorCodigo int,
	SucursalID int references TIENDA(SucursalID),
	UsuarioID int references USUARIO(UsuarioID),
	ClienteID int references CLIENTE(ClienteID),
	TipoDocumento varchar(50),
	CantidadProducto int,
	CantidadTotal int,
	TotalCosto  decimal(18,2),
	ImporteRecibido decimal(18,2),
	ImporteCambio decimal(18,2),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go


-- (12) TABLA DETALLE_VENTA
if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'DETALLE_VENTA')
create table DETALLE_VENTA(
	DetalleVentaID int primary key identity(1,1),
	VentaID int references VENTA(VentaID),
	ProductoID int references PRODUCTO(ProductoID),
	Cantidad int,
	PrecioUnidad decimal(18,2),
	ImporteTotal decimal(18,2),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
)

go
