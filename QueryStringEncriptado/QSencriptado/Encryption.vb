Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Security.Cryptography
Imports System.Globalization
Imports System.Web

'********************************************
'http://www.obviex.com/samples/encryption.aspx
'********************************************

Public Class Encryption
    Private rsa As RSACryptoServiceProvider
    Private rc2 As RC2CryptoServiceProvider
    Const saltValue As String = "s@1tValue"         ' can be any string
    Const hashAlgorithm As String = "MD5"          ' can be "MD5"
    Const passwordIterations As Integer = 1          ' can be any number
    Const initVector As String = "@1B2c3D4e5F6g7H8" ' must be 16 bytes
    Const keySize As Integer = 192                   ' can be 192 or 128

    Private Sub New()
    End Sub

    Public Shared Function EncryptQueryString(ByVal queryString As QueryString) As QueryString
        Dim newQueryString As QueryString = New QueryString
        Dim nm As String = String.Empty
        Dim val As String = String.Empty
        For Each name As String In queryString
            nm = name
            val = System.Web.HttpUtility.UrlEncode(queryString(name))
            newQueryString.Add(Encryption.Encrypt(nm, Date.Today.ToString), Encryption.Encrypt(val, Date.Today.ToString))
        Next
        Return newQueryString
    End Function

    Public Shared Function DecryptQueryString(ByVal queryString As QueryString) As QueryString
        Dim newQueryString As QueryString = New QueryString
        Dim nm As String
        Dim val As String
        For Each name As String In queryString
            nm = Encryption.Decrypt(name, Date.Today.ToString)
            val = System.Web.HttpUtility.UrlDecode(Encryption.Decrypt(queryString(name), Date.Today.ToString))
            newQueryString.Add(nm, val)
        Next
        Return newQueryString
    End Function


    'Private Shared Function EncryptString(ByVal InputString As String, ByVal SecretKey As String, Optional ByVal CyphMode As CipherMode = CipherMode.CBC) As String
    '    Dim Des As New TripleDESCryptoServiceProvider
    '    'Put the string into a byte array
    '    Dim InputbyteArray() As Byte = Encoding.UTF8.GetBytes(InputString)
    '    'Create the crypto objects, with the key, as passed in
    '    Dim hashMD5 As New MD5CryptoServiceProvider
    '    Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(SecretKey))
    '    Des.Mode = CyphMode
    '    Des.Padding = PaddingMode.Zeros

    '    Dim ms As MemoryStream = New MemoryStream
    '    Dim cs As CryptoStream = New CryptoStream(ms, Des.CreateEncryptor(), CryptoStreamMode.Write)
    '    'Write the byte array into the crypto stream
    '    '(It will end up in the memory stream)
    '    cs.Write(InputbyteArray, 0, InputbyteArray.Length)
    '    cs.FlushFinalBlock()
    '    'Get the data back from the memory stream, and into a string
    '    Dim ret As StringBuilder = New StringBuilder
    '    Dim b() As Byte = ms.ToArray
    '    ms.Close()
    '    Dim I As Integer
    '    For I = 0 To UBound(b)
    '        'Format as hex
    '        ret.AppendFormat("{0:X2}", b(I))
    '    Next

    '    Return ret.ToString.ToLower
    'End Function

    'Private Shared Function DecryptString(ByVal InputString As String, ByVal SecretKey As String, Optional ByVal CyphMode As CipherMode = CipherMode.CBC) As String
    '    If InputString = String.Empty Then
    '        Return ""
    '    Else
    '        Dim Des As New TripleDESCryptoServiceProvider
    '        'Put the string into a byte array
    '        Dim InputbyteArray(CType(InputString.Length / 2 - 1, Integer)) As Byte '= Encoding.UTF8.GetBytes(InputString)
    '        'Dim InputbyteArray() As Byte = Encoding.Unicode.GetBytes(InputString)
    '        'Create the crypto objects, with the key, as passed in
    '        Dim hashMD5 As New MD5CryptoServiceProvider

    '        Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(SecretKey))
    '        Des.Mode = CyphMode
    '        Des.Padding = PaddingMode.Zeros
    '        'Put the input string into the byte array

    '        Dim X As Integer

    '        For X = 0 To InputbyteArray.Length - 1
    '            Dim IJ As Int32 = (Convert.ToInt32(InputString.Substring(X * 2, 2), 16))
    '            Dim BT As New ComponentModel.ByteConverter
    '            InputbyteArray(X) = New Byte
    '            InputbyteArray(X) = CType(BT.ConvertTo(IJ, GetType(Byte)), Byte)
    '        Next

    '        Dim ms As MemoryStream = New MemoryStream
    '        Dim cs As CryptoStream = New CryptoStream(ms, Des.CreateDecryptor(), _
    '        CryptoStreamMode.Write)

    '        'Flush the data through the crypto stream into the memory stream
    '        cs.Write(InputbyteArray, 0, InputbyteArray.Length)
    '        cs.FlushFinalBlock()

    '        '//Get the decrypted data back from the memory stream
    '        Dim ret As StringBuilder = New StringBuilder
    '        Dim B() As Byte = ms.ToArray

    '        ms.Close()

    '        Dim I As Integer

    '        For I = 0 To UBound(B)
    '            ret.Append(Chr(B(I)))
    '        Next

    '        Return ret.ToString()
    '    End If
    'End Function

    Private Shared Function Encrypt(ByVal plainText As String, ByVal passPhrase As String) As String

        ' Convert strings into byte arrays.
        ' Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8 
        ' encoding.

        Dim initVectorBytes As Byte()
        initVectorBytes = Encoding.ASCII.GetBytes(initVector)

        Dim saltValueBytes As Byte()
        saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

        ' Convert our plaintext into a byte array.
        ' Let us assume that plaintext contains UTF8-encoded characters.
        Dim plainTextBytes As Byte()
        plainTextBytes = Encoding.UTF8.GetBytes(plainText)

        ' First, we must create a password, from which the key will be derived.
        ' This password will be generated from the specified passphrase and 
        ' salt value. The password will be created using the specified hash 
        ' algorithm. Password creation can be done in several iterations.
        Dim password As PasswordDeriveBytes

        password = New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte()
        keyBytes = password.GetBytes(keySize / 8)

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As RijndaelManaged
        symmetricKey = New RijndaelManaged()

        symmetricKey.Padding = PaddingMode.Zeros

        ' It is reasonable to set encryption mode to Cipher Block Chaining
        ' (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate encryptor from the existing key bytes and initialization 
        ' vector. Key size will be defined based on the number of the key 
        ' bytes.
        Dim encryptor As ICryptoTransform
        encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As MemoryStream
        memoryStream = New MemoryStream()

        ' Define cryptographic stream (always use Write mode for encryption).
        Dim cryptoStream As CryptoStream
        cryptoStream = New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
        ' Start encrypting.
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)

        ' Finish encrypting.
        cryptoStream.FlushFinalBlock()

        ' Convert our encrypted data from a memory stream into a byte array.
        Dim cipherTextBytes As Byte()
        cipherTextBytes = memoryStream.ToArray()

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()



        ' Convert encrypted data into a base64-encoded string.
        'Dim cipherText As String = Convert.ToBase64String(cipherTextBytes)
        'Return cipherText

        Dim ret As StringBuilder = New StringBuilder
        Dim B() As Byte = cipherTextBytes
        Dim I As Integer
        For I = 0 To B.Length - 1
            ret.AppendFormat("{0:X2}", B(I))
        Next

        Return ret.ToString()
    End Function



    Public Shared Function Decrypt(ByVal cipherText As String, ByVal passPhrase As String) As String
        ' Convert strings defining encryption key characteristics into byte
        ' arrays. Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8
        ' encoding.
        If cipherText = String.Empty Then
            Return ""
        Else
            Dim initVectorBytes As Byte()
            initVectorBytes = Encoding.ASCII.GetBytes(initVector)

            Dim saltValueBytes As Byte()
            saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

            ' Convert our ciphertext into a byte array.
            'Dim cipherTextBytes As Byte()
            'cipherTextBytes = Convert.FromBase64String(cipherText)
            'Dim cipherTextBytes() As Byte = Encoding.UTF8.GetBytes(cipherText)

            Dim cipherTextBytes(Convert.ToInt32(cipherText.Length / 2 - 1)) As Byte '= Encoding.UTF8.GetBytes(InputString)
            Dim X As Integer

            For X = 0 To cipherTextBytes.Length - 1
                Dim IJ As Int32 = (Convert.ToInt32(cipherText.Substring(X * 2, 2), 16))
                Dim BT As New ComponentModel.ByteConverter
                cipherTextBytes(X) = New Byte
                cipherTextBytes(X) = CType(BT.ConvertTo(IJ, GetType(Byte)), Byte)
            Next



            ' First, we must create a password, from which the key will be 
            ' derived. This password will be generated from the specified 
            ' passphrase and salt value. The password will be created using
            ' the specified hash algorithm. Password creation can be done in
            ' several iterations.
            Dim password As PasswordDeriveBytes
            password = New PasswordDeriveBytes(passPhrase, _
                                                saltValueBytes, _
                                                hashAlgorithm, _
                                                passwordIterations)

            ' Use the password to generate pseudo-random bytes for the encryption
            ' key. Specify the size of the key in bytes (instead of bits).
            Dim keyBytes As Byte()
            keyBytes = password.GetBytes(keySize / 8)

            ' Create uninitialized Rijndael encryption object.
            Dim symmetricKey As RijndaelManaged
            symmetricKey = New RijndaelManaged()

            symmetricKey.Padding = PaddingMode.Zeros

            ' It is reasonable to set encryption mode to Cipher Block Chaining
            ' (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC

            ' Generate decryptor from the existing key bytes and initialization 
            ' vector. Key size will be defined based on the number of the key 
            ' bytes.
            Dim decryptor As ICryptoTransform
            decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

            ' Define memory stream which will be used to hold encrypted data.
            Dim memoryStream As MemoryStream
            memoryStream = New MemoryStream(cipherTextBytes)

            ' Define memory stream which will be used to hold encrypted data.
            Dim cryptoStream As CryptoStream
            cryptoStream = New CryptoStream(memoryStream, _
                                            decryptor, _
                                            CryptoStreamMode.Read)

            ' Since at this point we don't know what the size of decrypted data
            ' will be, allocate the buffer long enough to hold ciphertext;
            ' plaintext is never longer than ciphertext.
            Dim plainTextBytes(cipherTextBytes.Length) As Byte

            ' Start decrypting.
            Dim decryptedByteCount As Integer
            decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

            Dim ret As StringBuilder = New StringBuilder
            Dim B() As Byte = memoryStream.ToArray

            ' Close both streams.
            memoryStream.Close()
            cryptoStream.Close()


            'Dim I As Integer
            'For I = 0 To UBound(B)
            '    ret.Append(Chr(B(I)))
            'Next

            'Return ret.ToString()



            ' Convert decrypted data into a string. 
            ' Let us assume that the original plaintext string was UTF8-encoded.
            Dim plainText As String
            plainText = Encoding.UTF8.GetString(plainTextBytes, _
                                                  0, _
                                                  decryptedByteCount)
            ' Return decrypted string.
            Decrypt = plainText
        End If
    End Function



End Class
