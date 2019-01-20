namespace SamFirm
{
    using System;

    public enum Error
    {
        AutoFetchError = 5,
        DecryptError = 3,
        DownloadError = 4,
        FIPSComplianceError = 800,
        Generic = 1,
        NoError = 0,
        NullResponse = 0x385,
        ResponseStreamError = 900,
        UpdateCheckError = 2
    }
}

