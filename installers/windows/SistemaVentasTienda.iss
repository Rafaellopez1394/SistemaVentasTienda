; Inno Setup Script for SistemaVentasTienda
#define MyAppName "SistemaVentasTienda"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Rafael Lopez"
#define MyAppExeName ""

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={commonpf}\{#MyAppName}
DisableDirPage=no
OutputDir=.
OutputBaseFilename=SistemaVentasTienda_Installer
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
DisableStartupPrompt=yes

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Files]
; Copy web app
Source: "..\..\VentasWeb\*"; DestDir: "{app}\VentasWeb"; Flags: recursesubdirs
; Copy SQL utility scripts
Source: "..\..\Utilidad\SQL Server\*"; DestDir: "{app}\SQL"; Flags: recursesubdirs
; Copy installers scripts
Source: "Install-Database.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "Configure-IIS.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "Update-WebConfig.ps1"; DestDir: "{app}"; Flags: ignoreversion
 Source: "Download-And-Install-SQLExpress.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "Ensure-Database.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "Silent-Install.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\productos_actuales.txt"; DestDir: "{app}"; Flags: ignoreversion

[Run]
; Try to install SQL Server Express & sqlcmd silently if missing
Filename: "powershell"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Download-And-Install-SQLExpress.ps1"" -InstanceName ""SQLEXPRESS"" -CustomUrl ""{code:GetSqlInstallerUrl}"""; StatusMsg: "Verificando/instalando SQL Server Express..."; Flags: runminimized
; Ensure database exists early
Filename: "powershell"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Ensure-Database.ps1"" -ServerInstance ""{code:GetSqlInstance}"" -DatabaseName ""{code:GetDatabaseName}"" -AuthType ""{code:GetAuthType}"" -SqlUser ""{code:GetSqlUser}"" -SqlPassword ""{code:GetSqlPassword}"""; StatusMsg: "Creando base de datos..."; Flags: runminimized
; Configure IIS site
Filename: "powershell"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Configure-IIS.ps1"" -PhysicalPath ""{app}\VentasWeb"" -Port ""{code:GetSitePort}"" -SiteName ""{code:GetSiteName}"" -AppPoolName ""{code:GetAppPoolName}"""; StatusMsg: "Configurando IIS..."; Flags: runminimized
; Update Web.config connection string using XML edit with installer inputs
Filename: "powershell"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Update-WebConfig.ps1"" -WebConfigPath ""{app}\VentasWeb\Web.config"" -ServerInstance ""{code:GetSqlInstance}"" -DatabaseName ""{code:GetDatabaseName}"" -AuthType ""{code:GetAuthType}"" -SqlUser ""{code:GetSqlUser}"" -SqlPassword ""{code:GetSqlPassword}"""; StatusMsg: "Actualizando conexión a SQL Server..."; Flags: runminimized
; Run database installer with installer inputs
Filename: "powershell"; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Install-Database.ps1"" -ServerInstance ""{code:GetSqlInstance}"" -DatabaseName ""{code:GetDatabaseName}"" -AuthType ""{code:GetAuthType}"" -SqlUser ""{code:GetSqlUser}"" -SqlPassword ""{code:GetSqlPassword}"" -ProductosFile ""{app}\productos_actuales.txt"""; StatusMsg: "Instalando base de datos..."; Flags: runminimized

[Icons]
Name: "{group}\{#MyAppName} (Sitio)"; Filename: "http://localhost:{code:GetSitePort}"; WorkingDir: "{app}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{group}\Instalación silenciosa (helper)"; Filename: "powershell"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File '{app}\\Silent-Install.ps1'"; WorkingDir: "{app}"

[Tasks]
; Optional: install SQL Server Express silently by downloading installer (commented)
;Name: "sqlserverexpress"; Description: "Instalar SQL Server Express"; Flags: unchecked

[Code]
var
	PageDB: TInputQueryWizardPage;
	AuthPage: TInputOptionWizardPage;
	CredPage: TInputQueryWizardPage;
	UrlPage: TInputQueryWizardPage;
	PortPage: TInputQueryWizardPage;

// Obtiene valor de parámetro de la línea de comando en formato /Clave=valor o -Clave=valor
function ParamValue(const Key, DefaultValue: String): String;
var
	I: Integer;
	Pfx1, Pfx2, Arg: String;
begin
	Result := DefaultValue;
	Pfx1 := '/' + Key + '=';
	Pfx2 := '-' + Key + '=';
	for I := 1 to ParamCount do
	begin
		Arg := ParamStr(I);
		if (Pos(Pfx1, Arg) = 1) or (Pos(Pfx2, Arg) = 1) then
		begin
			Result := Copy(Arg, Length(Pfx1) + 1, MaxInt);
			exit;
		end;
	end;
end;

procedure InitializeWizard;
begin
	PageDB := CreateInputQueryPage(wpSelectTasks,
		'Configuración de SQL Server',
		'Parámetros de base de datos',
		'Especifique la instancia de SQL y el nombre de la base de datos.');
	PageDB.Add('Instancia (p.ej. .\SQLEXPRESS):', False);
	PageDB.Add('Nombre de base de datos:', False);
	PageDB.Values[0] := ParamValue('SQLInstance', '.\SQLEXPRESS');
	PageDB.Values[1] := ParamValue('DatabaseName', 'DB_TIENDA');

	AuthPage := CreateInputOptionPage(PageDB.ID,
		'Autenticación', 'Tipo de autenticación SQL',
		'Elija el método de autenticación a usar.', True, False);
	AuthPage.Add('Windows');
	AuthPage.Add('SQL');
	if CompareText(ParamValue('AuthType', ''), 'SQL') = 0 then
		AuthPage.SelectedValueIndex := 1
	else
		AuthPage.SelectedValueIndex := 0;

	CredPage := CreateInputQueryPage(AuthPage.ID,
		'Credenciales SQL', 'Usuario y contraseña (si usa autenticación SQL)',
		'Si seleccionó "SQL", ingrese usuario y contraseña; con "Windows" se ignoran.');
	CredPage.Add('Usuario:', False);
	CredPage.Add('Contraseña:', True);
	CredPage.Values[0] := ParamValue('SqlUser', '');
	CredPage.Values[1] := ParamValue('SqlPassword', '');

	// Página opcional para URL de instalador SQL Express
	UrlPage := CreateInputQueryPage(CredPage.ID,
		'Instalador SQL Express (opcional)', 'URL personalizada del instalador',
		'Si la descarga automática falla, puede proporcionar una URL directa al instalador de SQL Express (.exe).');
	UrlPage.Add('URL del instalador:', False);
	UrlPage.Values[0] := ParamValue('SqlUrl', '');

	// Página de puerto del sitio IIS
	PortPage := CreateInputQueryPage(UrlPage.ID,
		'Configuración del sitio', 'Parámetros de IIS',
		'Especifique el puerto, nombre del sitio y del App Pool.');
	PortPage.Add('Puerto (ej. 8080):', False);
	PortPage.Add('Nombre del sitio:', False);
	PortPage.Add('Nombre del App Pool:', False);
	PortPage.Values[0] := ParamValue('SitePort', '8080');
	PortPage.Values[1] := ParamValue('SiteName', 'SistemaVentasTienda');
	PortPage.Values[2] := ParamValue('AppPoolName', 'SistemaVentasTiendaPool');
end;

function GetSqlInstance(Param: String): String;
begin
	Result := PageDB.Values[0];
end;

function GetDatabaseName(Param: String): String;
begin
	Result := PageDB.Values[1];
end;

function GetAuthType(Param: String): String;
begin
	if AuthPage.SelectedValueIndex = 1 then Result := 'SQL' else Result := 'Windows';
end;

function GetSqlUser(Param: String): String;
begin
	if GetAuthType('') = 'SQL' then Result := CredPage.Values[0] else Result := '';
end;

function GetSqlPassword(Param: String): String;
begin
	if GetAuthType('') = 'SQL' then Result := CredPage.Values[1] else Result := '';
end;

function GetSqlInstallerUrl(Param: String): String;
begin
	Result := UrlPage.Values[0];
end;

function GetSitePort(Param: String): String;
begin
	Result := PortPage.Values[0];
end;

function GetSiteName(Param: String): String;
begin
	Result := PortPage.Values[1];
end;

function GetAppPoolName(Param: String): String;
begin
	Result := PortPage.Values[2];
end;

function ValidateUrl(url: String): Boolean;
var
	ResultCode: Integer;
	Cmd: String;
begin
	if url = '' then
	begin
		Result := True;
		exit;
	end;
	// Valida con HEAD y hace fallback a GET con Range si HEAD falla; acepta 2xx/3xx
	Cmd := '-NoProfile -ExecutionPolicy Bypass -Command "'
		+ '$u=\'''+ url + '\'';'
		+ 'try { $r=Invoke-WebRequest -Uri $u -Method Head -UseBasicParsing -MaximumRedirection 5; $sc = $r.StatusCode } '
		+ 'catch { try { $r=Invoke-WebRequest -Uri $u -Method Get -UseBasicParsing -MaximumRedirection 5 -Headers @{Range=\''bytes=0-0\''}; $sc=$r.StatusCode } catch { exit 1 } } ;'
		+ 'if ($sc -ge 200 -and $sc -lt 400) { exit 0 } else { exit 1 }' + '"';
	if Exec('powershell', Cmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
		Result := (ResultCode = 0)
	else
		Result := False;
end;

function TestSqlConnection(AuthType, Instance, User, Password: String): Boolean;
var
	ResultCode: Integer;
	Cmd: String;
begin
	// Prueba rápida de conexión a la instancia usando la BD master
	Cmd := '-NoProfile -ExecutionPolicy Bypass -Command "'
		+ '$inst=\'''+ Instance + '\''; $db=\''master\''; $auth=\'''+ AuthType + '\''; $user=\'''+ User + '\''; $pwd=\'''+ Password + '\'';'
		+ ' $cs = \''Data Source=\'' + $inst + \'';Initial Catalog=\'' + $db + \'';\'';'
		+ ' if ($auth -eq \''Windows\'') { $cs += \''Integrated Security=True;\'' } else { $cs += \''Integrated Security=False;User ID=\'' + $user + \'';Password=\'' + $pwd + \'';\'' } ;'
		+ ' $cn = New-Object System.Data.SqlClient.SqlConnection $cs;'
		+ ' try { $cn.Open(); $cn.Close(); exit 0 } catch { exit 1 }' + '"';
	if Exec('powershell', Cmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
		Result := (ResultCode = 0)
	else
		Result := False;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
	Result := True;
	if CurPageID = UrlPage.ID then
	begin
		if not ValidateUrl(UrlPage.Values[0]) then
		begin
			if WizardSilent() then
				Log('URL de SQL Express inválida. Abortando instalación silenciosa.')
			else
				MsgBox('La URL del instalador de SQL Express no parece válida (estado o tamaño insuficiente). Verifique o déjelo vacío para usar descarga automática.', mbError, MB_OK);
			Result := False;
		end;
	end;
	// Validar conectividad a instancia al salir de credenciales
	if CurPageID = CredPage.ID then
	begin
		if not TestSqlConnection(GetAuthType(''), GetSqlInstance(''), GetSqlUser(''), GetSqlPassword('')) then
		begin
			if WizardSilent() then
			begin
				Log('Fallo de conexión a SQL Server. Abortando instalación silenciosa.');
				Result := False;
			end
			else if MsgBox('No se pudo conectar a la instancia especificada. ¿Desea continuar de todas formas?', mbConfirmation, MB_YESNO) = IDNO then
			begin
				Result := False;
			end;
		end;
	end;
	// Validar puerto numérico al salir de página de puerto
	if CurPageID = PortPage.ID then
	begin
		if (PortPage.Values[0] = '') or (StrToIntDef(PortPage.Values[0], -1) <= 0) then
		begin
			if WizardSilent() then
				Log('Puerto IIS inválido. Abortando instalación silenciosa.')
			else
				MsgBox('El puerto especificado no es válido. Ingrese un número mayor a 0.', mbError, MB_OK);
			Result := False;
		end;
		if (PortPage.Values[1] = '') then
		begin
			if WizardSilent() then
				Log('Nombre de sitio vacío. Abortando instalación silenciosa.')
			else
				MsgBox('Ingrese un nombre de sitio.', mbError, MB_OK);
			Result := False;
		end;
		if (PortPage.Values[2] = '') then
		begin
			if WizardSilent() then
				Log('Nombre de App Pool vacío. Abortando instalación silenciosa.')
			else
				MsgBox('Ingrese un nombre de App Pool.', mbError, MB_OK);
			Result := False;
		end;
	end;
end;
