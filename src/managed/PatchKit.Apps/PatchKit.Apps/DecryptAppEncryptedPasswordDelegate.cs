namespace PatchKit.Apps;

public delegate AppPassword? DecryptAppEncryptedPasswordDelegate(AppEncryptedPassword appEncryptedPassword, AppSecret appSecret);
