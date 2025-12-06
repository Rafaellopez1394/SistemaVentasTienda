go
use DBVENTAS_WEB
go
--************************ VALIDAMOS SI EXISTE EL PROCEDIMIENTO ************************--

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_LoginUsuario')
DROP PROCEDURE usp_LoginUsuario
go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerDetalleUsuario')
DROP PROCEDURE usp_ObtenerDetalleUsuario
go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerPermisos')
DROP PROCEDURE usp_ObtenerPermisos

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ActualizarPermisos')
DROP PROCEDURE usp_ActualizarPermisos

GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerUsuario')
DROP PROCEDURE usp_ObtenerUsuario

go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarUsuario')
DROP PROCEDURE usp_RegistrarUsuario

go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarUsuario')
DROP PROCEDURE usp_ModificarUsuario

go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarUsuario')
DROP PROCEDURE usp_EliminarUsuario

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerSucursal')
DROP PROCEDURE usp_ObtenerSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarSucursal')
DROP PROCEDURE usp_RegistrarSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarSucursal')
DROP PROCEDURE usp_ModificarSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarSucursal')
DROP PROCEDURE usp_EliminarSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerRoles')
DROP PROCEDURE usp_ObtenerRoles

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarRol')
DROP PROCEDURE usp_RegistrarRol

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarRol')
DROP PROCEDURE usp_ModificarRol

go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarRol')
DROP PROCEDURE usp_EliminarRol

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerProveedores')
DROP PROCEDURE usp_ObtenerProveedores

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarProveedor')
DROP PROCEDURE usp_RegistrarProveedor

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarProveedor')
DROP PROCEDURE usp_ModificarProveedor

go
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarProveedor')
DROP PROCEDURE usp_EliminarProveedor

go

--**************   PROCEDMIENTOS DE CATEGORIAS *****************************

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerCategorias')
DROP PROCEDURE usp_ObtenerCategorias

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarCategoria')
DROP PROCEDURE usp_RegistrarCategoria

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarCategoria')
DROP PROCEDURE usp_ModificarCategoria

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarCategoria')
DROP PROCEDURE usp_EliminarCategoria

go
--**************   PROCEDMIENTOS DE PRODUCTOS *****************************

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerProductos')
DROP PROCEDURE usp_ObtenerProductos

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarProducto')
DROP PROCEDURE usp_RegistrarProducto

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarProducto')
DROP PROCEDURE usp_ModificarProducto

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarProducto')
DROP PROCEDURE usp_EliminarProducto


GO
--**************   PROCEDMIENTOS DE PRODUCTO_TIENDA *****************************
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerProductoSucursal')
DROP PROCEDURE usp_ObtenerProductoSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarProductoSucursal')
DROP PROCEDURE usp_RegistrarProductoSucursal

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarProductoSucursal')
DROP PROCEDURE usp_ModificarProductoSucursal

go


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarProductoSucursal')
DROP PROCEDURE usp_EliminarProductoSucursal

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ControlarStock')
DROP PROCEDURE usp_ControlarStock

GO

--**************   PROCEDMIENTOS DE COMPRA *****************************
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarCompra')
DROP PROCEDURE usp_RegistrarCompra

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerDetalleCompra')
DROP PROCEDURE usp_ObtenerDetalleCompra

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerListaCompra')
DROP PROCEDURE usp_ObtenerListaCompra

GO


--**************   PROCEDMIENTOS DE VENTA *****************************
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarVenta')
DROP PROCEDURE usp_RegistrarVenta

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerDetalleVenta')
DROP PROCEDURE usp_ObtenerDetalleVenta

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerListaVenta')
DROP PROCEDURE usp_ObtenerListaVenta

GO

--**************   PROCEDMIENTOS PARA REPORTES *****************************
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_rptProductoSucursal')
DROP PROCEDURE usp_rptProductoSucursal

GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_rptVenta')
DROP PROCEDURE usp_rptVenta

GO
--**************   PROCEDMIENTOS PARA CLIENTE *****************************

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ObtenerCliente')
DROP PROCEDURE usp_ObtenerCliente

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_RegistrarCliente')
DROP PROCEDURE usp_RegistrarCliente

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarCliente')
DROP PROCEDURE usp_ModificarCliente

GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_EliminarCliente')
DROP PROCEDURE usp_EliminarCliente

GO

--PROCEDMIENTO PARA OBTENER USUARIO

create procedure usp_LoginUsuario(
@Correo varchar(60),
@Clave varchar(100),
@UsuarioID int output
)
as
begin
	set @UsuarioID = 0
	if exists(select * from USUARIO where Correo COLLATE Latin1_General_CS_AS = @Correo and Clave COLLATE Latin1_General_CS_AS = @Clave and Activo = 1)
		set @UsuarioID = (select top 1 UsuarioID from USUARIO where Correo  COLLATE Latin1_General_CS_AS = @Correo and Clave COLLATE Latin1_General_CS_AS = @Clave and Activo = 1)
end

go

--PROCEDMIENTO PARA OBTENER DETALLE USUARIO
create proc usp_ObtenerDetalleUsuario(
@UsuarioID int
)
as
begin

 select 
*,
(select * from TIENDA t
 where t.SucursalID = up.SucursalID
FOR XML PATH (''),TYPE) AS 'DetalleSucursal'
,
(select * from ROL r
 where r.RolID = up.RolID
FOR XML PATH (''),TYPE) AS 'DetalleRol'
,
 (
 select t.NombreMenu,t.Icono,
 (select sm.Nombre[NombreSubMenu],sm.Controlador,sm.Vista,sm.Icono,p.Activo
	 from PERMISOS p
	 inner join ROL r on r.RolID = p.RolID
	 inner join SUBMENU sm on sm.SubMenuID = p.SubMenuID
	 inner join MENU m on m.MenuID = sm.MenuID
	 inner join USUARIO u on u.RolID = r.RolID and u.UsuarioID = up.UsuarioID
	where sm.MenuID = t.MenuID
  FOR XML PATH ('SubMenu'),TYPE) AS 'DetalleSubMenu' 

 from (
 select distinct m.Nombre[NombreMenu],m.MenuID,m.Icono
 from PERMISOS p
 inner join ROL r on r.RolID = p.RolID
 inner join SUBMENU sm on sm.SubMenuID = p.SubMenuID
 inner join MENU m on m.MenuID = sm.MenuID
 inner join USUARIO u on u.RolID = r.RolID and u.UsuarioID = up.UsuarioID
 where p.Activo = 1) t
 FOR XML PATH ('Menu'),TYPE) AS 'DetalleMenu'  
 from USUARIO up
 where UP.UsuarioID = @UsuarioID
 FOR XML PATH(''), ROOT('Usuario')  

end

GO

--PROCEDMIENTO PARA OBTENER PERMISOS
create procedure usp_ObtenerPermisos(
@RolID int)
as
begin
select p.PermisosID,m.Nombre[Menu],sm.Nombre[SubMenu],p.Activo from PERMISOS p
inner join SUBMENU sm on sm.SubMenuID = p.SubMenuID
inner join MENU m on m.MenuID = sm.MenuID
where p.RolID = @RolID
end

go

--PROCEDIMIENTO PARA ACTUALIZAR PERMISOS
create procedure usp_ActualizarPermisos(
@Detalle xml,
@Resultado bit output
)
as
begin
begin try

	BEGIN TRANSACTION
	declare @permisos table(PermisosID int,activo bit)

	insert into @permisos(PermisosID,activo)
	select 
	PermisosID = Node.Data.value('(PermisosID)[1]','int'),
	activo = Node.Data.value('(Activo)[1]','bit')
	FROM @Detalle.nodes('/DETALLE/PERMISO') Node(Data)

	select * from @permisos

	update p set p.Activo = pe.activo from PERMISOS p
	inner join @permisos pe on pe.PermisosID = p.PermisosID

	COMMIT
	set @Resultado = 1

end try
begin catch
	ROLLBACK
	set @Resultado = 0
end catch
end

go

--PROCEDMIENTO PARA OBTENER USUARIOS
CREATE PROC usp_ObtenerUsuario
as
begin
 select u.UsuarioID,u.Nombres,u.Apellidos,u.Correo,u.Clave,u.SucursalID,u.RolID,u.Activo,u.FechaRegistro,r.Descripcion[DescripcionRol],u.Activo from USUARIO u
 inner join ROL r on r.RolID = u.RolID
end

go
--PROCEDIMIENTO PARA REGISTRAR USUARIO
CREATE PROC usp_RegistrarUsuario(
@Nombres varchar(50),
@Apellidos varchar(50),
@Correo varchar(50),
@Clave varchar(100),
@SucursalID int,
@RolID int,
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo)

		insert into USUARIO(Nombres,Apellidos,Correo,Clave,SucursalID,RolID) values (
		@Nombres,@Apellidos,@Correo,@Clave,@SucursalID,@RolID
		)
	ELSE
		SET @Resultado = 0
	
end

go
--PROCEDIMIENTO PARA MODIFICAR USUARIO
CREATE procedure usp_ModificarUsuario(
@UsuarioID int,
@Nombres varchar(50),
@Apellidos varchar(50),
@Correo varchar(50),
@Clave varchar(50),
@SucursalID int,
@RolID int,
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo and UsuarioID != @UsuarioID)
		
		update USUARIO set 
		Nombres = @Nombres,
		Apellidos = @Apellidos,
		Correo = @Correo,
		SucursalID = @SucursalID,
		RolID = @RolID,
		Activo = @Activo
		where UsuarioID = @UsuarioID

	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR USUARIO
create procedure usp_EliminarUsuario(
@UsuarioID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	--validamos que ningun usuario se encuentre relacionado a una tienda
	IF (not EXISTS (select c.* from COMPRA c 
		inner join USUARIO u on u.UsuarioID = c.UsuarioID 
		where u.UsuarioID = @UsuarioID
		) and 
		not EXISTS (select v.* from VENTA v 
		inner join USUARIO u on u.UsuarioID = v.UsuarioID 
		where u.UsuarioID = @UsuarioID
		)
	)
		delete from USUARIO where UsuarioID = @UsuarioID
	ELSE
		SET @Resultado = 0

end

go
--PROCEDMIENTO PARA OBTENER Sucursales
CREATE PROC usp_ObtenerSucursal
as
begin
 select SucursalID,Nombre,RFC,Direccion,Telefono,Activo from TIENDA
end

go
--PROCEDIMIENTO PARA GUARDAR TIENDA
CREATE PROC usp_RegistrarSucursal(
@Nombre varchar(50),
@RFC varchar(50),
@Direccion varchar(50),
@Telefono varchar(50),
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM TIENDA WHERE Nombre = @Nombre)

		insert into TIENDA(Nombre,RFC,Direccion,Telefono) values (
		@Nombre,@RFC,@Direccion,@Telefono
		)
	ELSE
		SET @Resultado = 0
	
end

go
--PROCEDIMIENTO PARA MODIFICAR TIENDA
create procedure usp_ModificarSucursal(
@SucursalID int,
@Nombre varchar(60),
@RFC varchar(60),
@Direccion varchar(60),
@Telefono varchar(60),
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM TIENDA WHERE Nombre =@Nombre and SucursalID != @SucursalID)
		
		update TIENDA set 
		Nombre = @Nombre,
		RFC = @RFC,
		Direccion = @Direccion,
		Telefono = @Telefono,
		Activo = @Activo
		where SucursalID = @SucursalID

	ELSE
		SET @Resultado = 0

end

go
--PROCEDIMIENTO PARA ELIMINAR TIENDA
create procedure usp_EliminarSucursal(
@SucursalID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	--validamos que la tienda no se encuentre asignada a un usuario y una venta
	IF not EXISTS (select * from USUARIO u
	inner join TIENDA t on t.SucursalID = u.SucursalID
	where t.SucursalID = @SucursalID)

		delete from TIENDA where SucursalID = @SucursalID

	ELSE
		SET @Resultado = 0

end

go

--PROCEDMIENTO PARA OBTENER ROLES
CREATE PROC usp_ObtenerRoles
as
begin
 select RolID, Descripcion,Activo from ROL
end

go

--PROCEDIMIENTO PARA GUARDAR ROL
CREATE PROC usp_RegistrarRol(
@Descripcion varchar(50),
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM ROL WHERE Descripcion = @Descripcion)
	begin
		declare @RolID int = 0
		insert into ROL(Descripcion) values (
		@Descripcion
		)
		set @RolID  = Scope_identity()

		insert into PERMISOS(RolID,SubMenuID,Activo)
		select @RolID, SubMenuID,0 from SUBMENU
	end
	ELSE
		SET @Resultado = 0
	
end


go

--PROCEDIMIENTO PARA MODIFICAR ROLES
create procedure usp_ModificarRol(
@RolID int,
@Descripcion varchar(60),
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM ROL WHERE Descripcion =@Descripcion and RolID != @RolID)
		
		update ROL set 
		Descripcion = @Descripcion,
		Activo = @Activo
		where RolID = @RolID
	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR ROL
create procedure usp_EliminarRol(
@RolID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	--validamos que el rol no se encuentre asignado a algun usuario
	IF not EXISTS (select * from USUARIO u
	inner join ROL r on r.RolID  = u.RolID
	where r.RolID = @RolID)
	begin	
		delete from PERMISOS where RolID = @RolID
		delete from ROL where RolID = @RolID
	end
	ELSE
		SET @Resultado = 0

end

go
--PROCEDMIENTO PARA OBTENER PROVEEDORES
CREATE PROC usp_ObtenerProveedores
as
begin
 select ProveedorID,RFC,RazonSocial,Telefono,Correo,Direccion,Activo from PROVEEDOR
end

go


--PROCEDIMIENTO PARA GUARDAR PROVEEDOR
CREATE PROC usp_RegistrarProveedor(
@RFC varchar(50),
@RazonSocial varchar(100),
@Telefono varchar(50),
@Correo varchar(50),
@Direccion varchar(50),
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PROVEEDOR WHERE RFC = @RFC)

		insert into PROVEEDOR(RFC,RazonSocial,Telefono,Correo,Direccion)
		values(@RFC,@RazonSocial,@Telefono,@Correo,@Direccion)

	ELSE
		SET @Resultado = 0
	
end

go


--PROCEDIMIENTO PARA MODIFICAR PROVEEDOR
create procedure usp_ModificarProveedor(
@ProveedorID int,
@RFC varchar(50),
@RazonSocial varchar(100),
@Telefono varchar(50),
@Correo varchar(50),
@Direccion varchar(50),
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PROVEEDOR WHERE RFC = @RFC and ProveedorID != @ProveedorID)
		
		update PROVEEDOR set 
		RFC = @RFC,
		RazonSocial = @RazonSocial,
		Telefono = @Telefono,
		Correo = @Correo,
		Direccion = @Direccion,
		Activo = @Activo
		where ProveedorID = @ProveedorID
	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR PROVEEDOR
create procedure usp_EliminarProveedor(
@ProveedorID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	--validamos que ningun proveedor se encuentre asignado a una compra
	IF not EXISTS (select top 1 * from COMPRA c
inner join PROVEEDOR p on p.ProveedorID = c.ProveedorID
where p.ProveedorID = @ProveedorID)

		delete from PROVEEDOR where ProveedorID = @ProveedorID
	ELSE
		SET @Resultado = 0

end

go


--PROCEDMIENTO PARA OBTENER CATEGORIA
CREATE PROC usp_ObtenerCategorias
as
begin
 select CategoriaID,Descripcion,Activo from CATEGORIA
end

go

--PROCEDIMIENTO PARA GUARDAR CATEGORIA
CREATE PROC usp_RegistrarCategoria(
@Descripcion varchar(50),
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)

		insert into CATEGORIA(Descripcion) values (
		@Descripcion
		)
	ELSE
		SET @Resultado = 0
	
end

go

--PROCEDIMIENTO PARA MODIFICAR CATEGORIA
create procedure usp_ModificarCategoria(
@CategoriaID int,
@Descripcion varchar(60),
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion =@Descripcion and CategoriaID != @CategoriaID)
		
		update CATEGORIA set 
		Descripcion = @Descripcion,
		Activo = @Activo
		where CategoriaID = @CategoriaID
	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR CATEGORIA
create procedure usp_EliminarCategoria(
@CategoriaID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	--validamos que ninguna categoria este relacionada a un producto
	IF not EXISTS (select * from CATEGORIA c
	inner join PRODUCTO p on c.CategoriaID  = p.CategoriaID
	where c.CategoriaID = @CategoriaID)

		delete from CATEGORIA where CategoriaID = @CategoriaID

	ELSE
		SET @Resultado = 0

end



go

--PROCEMIENTO PARA OBTENER PRODUCTO
CREATE PROC usp_ObtenerProductos
as
begin
 select ProductoID,Codigo,ValorCodigo,Nombre,p.Descripcion[DescripcionProducto],p.CategoriaID,p.Activo,c.Descripcion[DescripcionCategoria] from PRODUCTO p
 inner join CATEGORIA c on c.CategoriaID = p.CategoriaID

end

go

--PROCEDIMIENTO PARA GUARDAR PRODUCTO
CREATE PROC usp_RegistrarProducto(
@Nombre varchar(50),
@Descripcion varchar(50),
@CategoriaID int,
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PRODUCTO WHERE rtrim(ltrim(Nombre)) = rtrim(ltrim(@Nombre)))

		insert into PRODUCTO(Codigo,ValorCodigo,Nombre,Descripcion,CategoriaID) values (
		RIGHT('000000' + convert(varchar(max),(select isnull(max(ValorCodigo),0) + 1 from PRODUCTO)),6),(select isnull(max(ValorCodigo),0) + 1 from PRODUCTO),@Nombre,@Descripcion,@CategoriaID
		)
	ELSE
		SET @Resultado = 0
	
end

go

--PROCEDIMIENTO PARA MODIFICAR PRODUCTO
create procedure usp_ModificarProducto(
@ProductoID int,
@Nombre varchar(50),
@Descripcion varchar(50),
@CategoriaID int,
@Activo bit,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PRODUCTO WHERE rtrim(ltrim(Nombre)) = rtrim(ltrim(@Nombre)) and ProductoID != @ProductoID)
		
		update PRODUCTO set
		Nombre = @Nombre,
		Descripcion = @Descripcion,
		CategoriaID = @CategoriaID,
		Activo = @Activo
		where ProductoID = @ProductoID
	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR PRODUCTO
create procedure usp_EliminarProducto(
@ProductoID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	--validamos que ningun producto se encuentre relacionado a una tienda
	IF not EXISTS (select top 1 * from PRODUCTO_TIENDA PT
INNER JOIN PRODUCTO P ON P.ProductoID = PT.ProductoID
WHERE P.ProductoID = @ProductoID)
		delete from PRODUCTO where ProductoID = @ProductoID

	ELSE
		SET @Resultado = 0

end

GO
/********************* PRODUCTO_TIENDA **************************/


--PROCEDMIENTO PARA OBTENER PRODUCTO_TIENDA
CREATE PROC usp_ObtenerProductoSucursal
as
begin
 select pt.ProductoSucursalID,p.ProductoID,p.Codigo[CodigoProducto], p.Nombre[NombreProducto],p.Descripcion[DescripcionProducto],
 t.SucursalID,t.RFC,t.Nombre[NombreSucursal] ,t.Direccion[DireccionSucursal],pt.PrecioUnidadCompra,pt.PrecioUnidadVenta,pt.Stock,pt.Iniciado
 from PRODUCTO_TIENDA pt
 inner join PRODUCTO p on p.ProductoID = pt.ProductoID
 inner join TIENDA t on t.SucursalID = pt.SucursalID
end

go


--PROCEDIMIENTO PARA GUARDAR PRODUCTO_TIENDA
CREATE PROC usp_RegistrarProductoSucursal(
@ProductoID int,
@SucursalID int,
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PRODUCTO_TIENDA WHERE ProductoID = @ProductoID and SucursalID = @SucursalID)

		insert into PRODUCTO_TIENDA(ProductoID,SucursalID) values (
		@ProductoID,@SucursalID
		)
	ELSE
		SET @Resultado = 0
	
end

go

--PROCEDIMIENTO PARA MODIFICAR PRODUCTO_TIENDA
create procedure usp_ModificarProductoSucursal(
@ProductoSucursalID int,
@ProductoID varchar(60),
@SucursalID varchar(60),
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM PRODUCTO_TIENDA WHERE @ProductoSucursalID = @ProductoSucursalID and Iniciado = 1 )
		
		update PRODUCTO_TIENDA set 
		ProductoID = @ProductoID,
		SucursalID = @SucursalID
		where ProductoSucursalID = @ProductoSucursalID
	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA ELIMINAR PRODUCTO_TIENDA
create procedure usp_EliminarProductoSucursal(
@ProductoSucursalID int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1

	IF NOT EXISTS (SELECT * FROM PRODUCTO_TIENDA WHERE ProductoSucursalID = @ProductoSucursalID and Iniciado = 1 )

		delete from PRODUCTO_TIENDA where ProductoSucursalID = @ProductoSucursalID

	ELSE
		SET @Resultado = 0

end

go

--PROCEDIMIENTO PARA STOCK PRODUCTO_TIENDA

CREATE PROCEDURE usp_ControlarStock(
@ProductoID int,
@SucursalID int,
@Cantidad int,
@Restar bit,
@Resultado bit output)
as
begin
	set @Resultado = 1
	begin try
		if(@Restar = 1)
			update PRODUCTO_TIENDA set Stock = Stock - @Cantidad where ProductoID = @ProductoID and SucursalID = @SucursalID and Iniciado = 1
		else
			update PRODUCTO_TIENDA set Stock = Stock + @Cantidad where ProductoID = @ProductoID and SucursalID = @SucursalID and Iniciado = 1
	end try
	begin catch
		set @Resultado = 0
	end catch
end


go

/********************* COMPRA **************************/

--PROCEDIMIENTO PARA REGITRAR COMPRA
create procedure usp_RegistrarCompra(
@Detalle xml,
@Resultado bit output
)
as
begin


begin try

	BEGIN TRANSACTION

	declare @compra table(UsuarioID int,ProveedorID int,SucursalID int,totalcosto decimal(18,2))
	declare @detallecompra table(CompraID int,ProductoID int,cantidad int,preciounidadcompra decimal(18,2),preciounidadventa decimal(18,2),totalcosto decimal(18,2))
	declare @tiendaproducto table(SucursalID int,ProductoID int,cantidad int,preciounidadcompra decimal(18,2),preciounidadventa decimal(18,2),totalcosto decimal(18,2))

	 insert into @compra(UsuarioID,ProveedorID,SucursalID,totalcosto)
	 select 
	 UsuarioID = Node.Data.value('(UsuarioID)[1]','int'),
	 ProveedorID = Node.Data.value('(ProveedorID)[1]','int'),
	 SucursalID = Node.Data.value('(SucursalID)[1]','int'),
	 TotalCosto = Node.Data.value('(TotalCosto)[1]','decimal(18,2)')
	 FROM @Detalle.nodes('/DETALLE/COMPRA') Node(Data)
 
	 insert into @detallecompra(CompraID,ProductoID,cantidad,preciounidadcompra,preciounidadventa,totalcosto)
	 select 
	 CompraID = Node.Data.value('(CompraID)[1]','int'),
	 ProductoID = Node.Data.value('(ProductoID)[1]','int'),
	 Cantidad = Node.Data.value('(Cantidad)[1]','int'),
	 PrecioUnidadCompra = Node.Data.value('(PrecioUnidadCompra)[1]','decimal(18,2)'),
	 PrecioUnidadVenta = Node.Data.value('(PrecioUnidadVenta)[1]','decimal(18,2)'),
	 TotalCosto = Node.Data.value('(TotalCosto)[1]','decimal(18,2)')
	 FROM @Detalle.nodes('/DETALLE/DETALLE_COMPRA/DETALLE') Node(Data)

	 insert into @tiendaproducto(SucursalID,ProductoID,cantidad,preciounidadcompra,preciounidadventa,totalcosto)
	 select
	 (select top 1 SucursalID from @compra),
	 ProductoID,cantidad,preciounidadcompra,preciounidadventa,totalcosto
	 from @detallecompra


	 --******************* AREA DE TRABAJO *************************
	 declare @CompraID int = 0

	 insert into COMPRA(UsuarioID,ProveedorID,SucursalID,TotalCosto)
	 select UsuarioID,ProveedorID,SucursalID,totalcosto from @compra

	 set @CompraID = Scope_identity()
	 update @detallecompra set CompraID = @CompraID

	 insert into DETALLE_COMPRA(CompraID,ProductoID,Cantidad,PrecioUnitarioCompra,PrecioUnitarioVenta,TotalCosto)
	 select CompraID,ProductoID,cantidad,preciounidadcompra,preciounidadventa,totalcosto from @detallecompra

	 update pt set 
	 pt.PrecioUnidadCompra = tp.preciounidadcompra, 
	 pt.PrecioUnidadVenta = tp.preciounidadventa,
	 pt.Stock = pt.Stock + tp.cantidad,
	 pt.Iniciado = 1
	 from PRODUCTO_TIENDA pt
	 inner join @tiendaproducto tp on tp.SucursalID = pt.SucursalID and tp.ProductoID = pt.ProductoID

	 COMMIT
	 set @Resultado = 1

 end try
 begin catch
	ROLLBACK
	set @Resultado = 0
 end catch
end



GO

--PROCEDMIENTO PARA OBTENER DETALLE COMPRA
create proc usp_ObtenerDetalleCompra(
@CompraID int
)
as
begin


select  RIGHT('000000' + convert(varchar(max),c.CompraID),6)[Codigo],
CONVERT(char(10),c.FechaRegistro,103)[FechaCompra],
CONVERT(decimal(10,2), c.TotalCosto)[TotalCosto],

(select p.RFC,p.RazonSocial from PROVEEDOR P
 where p.ProveedorID = c.ProveedorID
FOR XML PATH (''),TYPE) AS 'DETALLE_PROVEEDOR',

(select T.RFC, T.Nombre, T.Direccion from TIENDA T
 where T.SucursalID = c.SucursalID
FOR XML PATH (''),TYPE) AS 'DETALLE_TIENDA',

(select convert(int, dc.Cantidad)[Cantidad],concat(p.Nombre,' - ',p.Descripcion)[NombreProducto],
CONVERT(decimal(10,2),dc.PrecioUnitarioCompra)PrecioUnitarioCompra,
CONVERT(decimal(10,2),dc.TotalCosto)[TotalCosto] 
from DETALLE_COMPRA dc
inner join PRODUCTO p on p.ProductoID = dc.ProductoID
where dc.CompraID = c.CompraID
FOR XML PATH ('PRODUCTO'),TYPE) AS 'DETALLE_PRODUCTO'

from COMPRA c
where c.CompraID =@CompraID
FOR XML PATH(''), ROOT('DETALLE_COMPRA') 
end

go


--PROCEDMIENTO PARA OBTENER LISTA COMPRA
create procedure usp_ObtenerListaCompra(
@FechaInicio date,
@FechaFin date,
@ProveedorID int = 0 ,
@SucursalID int = 0
)
as
begin
SET DATEFORMAT dmy;
select c.CompraID,RIGHT('000000' + convert(varchar(max),c.CompraID),6)[NumeroCompra], p.RazonSocial,t.Nombre,
convert(char(10),c.FechaRegistro,103)[FechaCompra],c.TotalCosto from COMPRA c
inner join PROVEEDOR p on p.ProveedorID = c.ProveedorID
inner join TIENDA t on t.SucursalID = c.SucursalID
where 
CONVERT(date,c.FechaRegistro) between @FechaInicio and @FechaFin and
p.ProveedorID = iif(@ProveedorID = 0,p.ProveedorID,@ProveedorID) and
t.SucursalID =iif(@SucursalID = 0,t.SucursalID,@SucursalID) 

end


GO

/********************* VENTA **************************/

--PROCEDIMIENTO PARA REGITRAR VENTA
create procedure usp_RegistrarVenta(
@Detalle xml,
@Resultado int output
)
as
begin


begin try

	BEGIN TRANSACTION
	
	declare @cliente table (tipodocumento varchar(50),numerodocumento varchar(50),nombre varchar(100),direccion varchar(100),telefono varchar(50))
	declare @venta table (SucursalID int,UsuarioID int,ClienteID int default 0,tipodocumento varchar(50),cantidadproducto int,cantidadtotal int,totalcosto decimal(18,2),importerecibido decimal(18,2),importecambio decimal(18,2))
	declare @detalleventa table (VentaID int,ProductoID int,cantidad int,preciounidad decimal(18,2),importetotal decimal(18,2))

	insert into @cliente(tipodocumento,numerodocumento,nombre,direccion,telefono)
		 select 
		 tipodocumento = Node.Data.value('(TipoDocumento)[1]','varchar(50)'),
		 numerodocumento = Node.Data.value('(NumeroDocumento)[1]','varchar(50)'),
		 nombre = Node.Data.value('(Nombre)[1]','varchar(100)'),
		 direccion = Node.Data.value('(Direccion)[1]','varchar(100)'),
		 telefono = Node.Data.value('(Telefono)[1]','varchar(50)')
		 FROM @Detalle.nodes('/DETALLE/DETALLE_CLIENTE/DATOS') Node(Data)

	insert into @venta(SucursalID,UsuarioID,ClienteID,tipodocumento,cantidadproducto,cantidadtotal,totalcosto,importerecibido,importecambio)
	select 
		 SucursalID = Node.Data.value('(SucursalID)[1]','varchar(50)'),
		 UsuarioID = Node.Data.value('(UsuarioID)[1]','varchar(50)'),
		 ClienteID = Node.Data.value('(ClienteID)[1]','varchar(100)'),
		 TipoDocumento = Node.Data.value('(TipoDocumento)[1]','varchar(100)'),
		 CantidadProducto = Node.Data.value('(CantidadProducto)[1]','varchar(50)'),
		 CantidadTotal = Node.Data.value('(CantidadTotal)[1]','varchar(50)'),
		 TotalCosto = Node.Data.value('(TotalCosto)[1]','decimal(18,2)'),
		 ImporteRecibido = Node.Data.value('(ImporteRecibido)[1]','decimal(18,2)'),
		 ImporteCambio = Node.Data.value('(ImporteCambio)[1]','decimal(18,2)')
		 FROM @Detalle.nodes('/DETALLE/VENTA') Node(Data)

	insert into @detalleventa(VentaID,ProductoID,cantidad,preciounidad,importetotal)
		 select 
		 VentaID = Node.Data.value('(VentaID)[1]','int'),
		 ProductoID = Node.Data.value('(ProductoID)[1]','int'),
		 Cantidad = Node.Data.value('(Cantidad)[1]','int'),
		 PrecioUnidad = Node.Data.value('(PrecioUnidad)[1]','decimal(18,2)'),
		 ImporteTotal = Node.Data.value('(ImporteTotal)[1]','decimal(18,2)')
		 FROM @Detalle.nodes('/DETALLE/DETALLE_VENTA/DATOS') Node(Data)

	--******************* AREA DE TRABAJO *************************
	declare @identity as table(ID int)

	if not exists(select * from CLIENTE where numeroDocumento = (select numerodocumento from @cliente))
	insert into CLIENTE(TipoDocumento,NumeroDocumento,Nombre,Direccion,Telefono)
	output inserted.ClienteID into @identity
	select tipodocumento,numerodocumento,nombre,direccion,telefono from @cliente
	else 
	 insert into @identity(ID)
	 select ClienteID from CLIENTE where numeroDocumento = (select numerodocumento from @cliente)

	update @venta set ClienteID = (select ID from @identity)
	delete from @identity

	insert into VENTA(Codigo,ValorCodigo,SucursalID,UsuarioID,ClienteID,TipoDocumento,CantidadProducto,CantidadTotal,TotalCosto,ImporteRecibido,ImporteCambio)
	output inserted.VentaID into @identity
	select 
	RIGHT('000000' + convert(varchar(max),(select isnull(max(ValorCodigo),0) + 1 from VENTA) ),6),
	(select isnull(max(ValorCodigo),0) + 1 from VENTA),
	SucursalID,UsuarioID,ClienteID,tipodocumento,cantidadproducto,cantidadtotal,totalcosto,importerecibido,importecambio
	from @venta

	update @detalleventa set VentaID = (select ID from @identity)

	insert into DETALLE_VENTA(VentaID,ProductoID,Cantidad,PrecioUnidad,ImporteTotal)
	select VentaID,ProductoID,cantidad,preciounidad,importetotal from @detalleventa


	 COMMIT
	 set @Resultado = (select ID from @identity)

 end try
 begin catch
	ROLLBACK
	set @Resultado = 0
 end catch
end

GO

--PROCEDMIENTO PARA OBTENER DETALLE VENTA
create proc usp_ObtenerDetalleVenta(
@VentaID int
)
as
begin


select V.TipoDocumento, V.Codigo,
CONVERT(decimal(10,2), V.TotalCosto)[TotalCosto],
CONVERT(decimal(10,2),V.ImporteRecibido)[ImporteRecibido],
CONVERT(decimal(10,2), V.ImporteCambio)[ImporteCambio],
convert(char(10),v.fechaRegistro,103)[FechaRegistro],
(select u.Nombres,u.Apellidos from USUARIO U
 where U.UsuarioID = v.UsuarioID
FOR XML PATH (''),TYPE) AS 'DETALLE_USUARIO',

(select T.RFC, T.Nombre, T.Direccion from TIENDA T
 where T.SucursalID = V.SucursalID
FOR XML PATH (''),TYPE) AS 'DETALLE_TIENDA',

(select C.Nombre,C.Direccion,C.NumeroDocumento,C.Telefono from CLIENTE c
 where c.ClienteID = V.ClienteID
FOR XML PATH (''),TYPE) AS 'DETALLE_CLIENTE',

(select dv.Cantidad,concat(p.Nombre,'-',p.Descripcion)[NombreProducto],
CONVERT(decimal(10,2),dv.PrecioUnidad)[PrecioUnidad],
CONVERT(decimal(10,2),dv.ImporteTotal)[ImporteTotal] 
from DETALLE_VENTA dv
inner join PRODUCTO p on p.ProductoID = dv.ProductoID
where dv.VentaID = v.VentaID
FOR XML PATH ('PRODUCTO'),TYPE) AS 'DETALLE_PRODUCTO'

from VENTA v
where v.VentaID = @VentaID
FOR XML PATH(''), ROOT('DETALLE_VENTA') 

end

GO

--PROCEDMIENTO PARA OBTENER LISTA VENTA

create procedure usp_ObtenerListaVenta(
@Codigo varchar(50) = '',
@FechaInicio date,
@FechaFin date,
@NumeroDocumento varchar(50) = '',
@Nombre varchar(100) = ''
)
as
begin
SET DATEFORMAT dmy;
select v.VentaID, v.TipoDocumento,v.Codigo,v.FechaRegistro,c.NumeroDocumento,c.Nombre,v.TotalCosto from VENTA v 
inner join CLIENTE c on c.ClienteID = v.ClienteID
where 
v.Codigo = iif(@Codigo='',v.Codigo,@Codigo) and
CONVERT(date,v.FechaRegistro) between @FechaInicio and @FechaFin and
c.NumeroDocumento like CONCAT('%',@NumeroDocumento,'%') and
c.Nombre like CONCAT('%',@Nombre,'%')

end

go
--PROCEDMIENTO PARA REPORTE TIENDA PRODUCTO

 create procedure usp_rptProductoSucursal(
 @SucursalID int = 0,
 @Codigo varchar(50)
 )
 as
 begin
 select 
 t.RFC[RFC Sucursal],t.Nombre[Nombre Sucursal],t.Direccion[Direccion Sucursal],
 p.Codigo[Codigo Producto],p.Nombre[Nombre Producto],p.Descripcion[Descripcion Producto],
 pt.Stock[Stock en tienda],convert(decimal(10,2),pt.PrecioUnidadCompra)[Precio Compra],convert(decimal(10,2),pt.PrecioUnidadVenta)[Precio Venta]
 from PRODUCTO_TIENDA pt
 inner join PRODUCTO p on p.ProductoID = pt.ProductoID
 inner join TIENDA t on t.SucursalID = pt.SucursalID
 where pt.SucursalID = iif(@SucursalID = 0,pt.SucursalID,@SucursalID) and
 p.Codigo like '%' + @Codigo + '%'
 end

 go

 --PROCEDMIENTO PARA REPORTE VENTA

create procedure usp_rptVenta (
@FechaInicio date,
@FechaFin date,
@SucursalID int = 0
)
as
begin

 select convert(char(10), v.FechaRegistro ,103)[Fecha Venta],v.Codigo[Numero Documento],v.TipoDocumento[Tipo Documento],
 t.Nombre[Nombre Sucursal], t.RFC[RFC Sucursal],
 concat(u.Nombres,' ', u.apellidos)[Nombre Empleado],
 v.CantidadTotal[Cantidad Unidades Vendidas],v.CantidadProducto[Cantidad Productos],v.TotalCosto[Total Venta] 
 from VENTA v
 inner join TIENDA t on t.SucursalID = v.SucursalID
 inner join USUARIO u on u.UsuarioID = v.UsuarioID
 where 
 CONVERT(date,v.FechaRegistro) between @FechaInicio and @FechaFin 
 and v.SucursalID = iif(@SucursalID =0 ,v.SucursalID,@SucursalID)
end

go


--PROCEDMIENTO PARA OBTENER USUARIOS
CREATE PROC usp_ObtenerCliente
as
begin
  select ClienteID,TipoDocumento,NumeroDocumento,Nombre,Direccion,Telefono,Activo from CLIENTE
end


go

--PROCEDIMIENTO PARA GUARDAR CLIENTE
CREATE PROC usp_RegistrarCliente(
@TipoDocumento varchar(100),
@NumeroDocumento varchar(100),
@Nombre varchar(100),
@Direccion varchar(100),
@Telefono varchar(100),
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE NumeroDocumento = @NumeroDocumento)
	begin
		insert into cliente(TipoDocumento,NumeroDocumento,Nombre,Direccion,Telefono) values
		(@TipoDocumento,@NumeroDocumento,@Nombre,@Direccion,@Telefono)
	end
	ELSE
		SET @Resultado = 0
	
end

go
--PROCEDIMIENTO PARA MODIFICAR CLIENTE
CREATE PROC usp_ModificarCliente(
@ClienteID int,
@TipoDocumento varchar(100),
@NumeroDocumento varchar(100),
@Nombre varchar(100),
@Direccion varchar(100),
@Telefono varchar(100),
@Activo bit,
@Resultado bit output
)as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE NumeroDocumento = @NumeroDocumento and ClienteID != @ClienteID)
	begin
		update CLIENTE set 
		TipoDocumento = @TipoDocumento,
		NumeroDocumento = @NumeroDocumento,
		Nombre = @Nombre,
		Direccion = @Direccion,
		Telefono = @Telefono,
		Activo = @Activo
		where ClienteID = @ClienteID
	end
	ELSE
		SET @Resultado = 0
	
end

go

--PROCEDIMIENTO PARA ELIMINAR CLIENTE
create procedure usp_EliminarCliente(
@ClienteID int,
@Resultado bit output
)
as
begin
	set @Resultado = 1
	begin try
		delete from CLIENTE where ClienteID = @ClienteID

	end try
	begin catch
		set @Resultado = 0
	end catch
end
