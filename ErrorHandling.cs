﻿
namespace Synology
{
    public static class ErrorHandling
    {
        public static string GetMessageFromCode(int InCode)
        {
            switch (InCode) 
            {
                // Custom errors
                case 1: return "Connection timeout";
                case 2: return "Failed to parse http response";

                // From the official docs
                case 100: return "Unknown error";
                case 101: return "No parameter of API, method or version";
                case 102: return "The requested API does not exist";
                case 103: return "The requested method does not exist";
                case 104: return "The requested version does not support the functionality";
                case 105: return "The logged in session does not have permission";
                case 106: return "Session timeout";
                case 107: return "Session interrupted by duplicate login";
                case 119: return "SID not found";
                case 400: return "Invalid parameter of file operation";
                case 401: return "Unknown error of file operation";
                case 402: return "System is too busy";
                case 403: return "Invalid user does this file operation";
                case 404: return "Invalid group does this file operation";
                case 405: return "Invalid user and group does this file operation";
                case 406: return "Can't get user/group information from the account server";
                case 407: return "Operation not permitted";
                case 408: return "No such file or directory";
                case 409: return "Non-supported file system";
                case 410: return "Failed to connect internet-based file system (e.g., CIFS)";
                case 411: return "Read-only file system";
                case 412: return "Filename too long in the non-encrypted file system";
                case 413: return "Filename too long in the encrypted file system";
                case 414: return "File already exists";
                case 415: return "Disk quota exceeded";
                case 416: return "No space left on device";
                case 417: return "Input/output error";
                case 418: return "Illegal name or path";
                case 419: return "Illegal file name";
                case 420: return "Illegal file name on FAT file system";
                case 421: return "Device or resource busy";
                case 599: return "No such task of the file operation";

                // Upload
                case 1800: return "There is no Content-Length information in the HTTP header or the received size doesn't match the value of Content-Length information in the HTTP header.";
                case 1801: return "Wait too long, no date can be received from client (Default maximum wait time is 3600 seconds).";
                case 1802: return "No filename information in the last part of file content.";
                case 1803: return "Upload connection is cancelled.";
                case 1804: return "Failed to upload oversized file to FAT file system.";
                case 1805: return "Can't overwrite or skip the existing file, if no overwrite parameter is given.";

            }
            return "Unknown error code"; 
        }
    }
}
