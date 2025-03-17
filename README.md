# ProxyManage

#### 1、更改git配置

```sh
git config core.autocrlf false
```

#### 2、使用前需完成的配置

1. ##### **配置 Secrets**

   在 GitHub 仓库的 `Settings > Secrets` 中添加：

   - `WINDOWS_PFX_BASE64`: PFX 证书的 Base64 编码字符串。
   - `WINDOWS_PFX_PASSWORD`: PFX 证书密码。

2. ##### **生成有效的代码签名证书**

   若你使用自签名证书，需确保其为代码签名证书。通过 PowerShell 生成自签名证书：

   ```shell
   # 生成自签名证书（代码签名用途）
   New-SelfSignedCertificate `
     -Subject "CN=YourAppName" `
     -Type CodeSigning `
     -KeyUsage DigitalSignature `
     -CertStoreLocation "Cert:\CurrentUser\My" `
     -KeyExportPolicy Exportable `
     -KeySpec Signature
   ```

3. ##### **导出 PFX 文件并配置 GitHub Secrets**

   1. ###### **生成 PFX 证书的 Base64 编码**

      在本地 PowerShell 中运行：

      ```powershell
      $pfxBytes = Get-Content -Path "./ProxyManage/ProxyManage_TemporaryKey.pfx" -Encoding Byte
      [System.Convert]::ToBase64String($pfxBytes) | Out-File "pfx-base64.txt"
      ```

      将生成的文本内容复制到 Secrets。

4. 发布

   ```powershell
   dotnet publish -f net8.0-windows10.0.19041.0 -c Release
   ```

