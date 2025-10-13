using Microsoft.AspNetCore.Http;

namespace ContractorBackend.Application.Services
{
    /// <summary>
    /// check Binary file type
    /// </summary>
    public static class CheckFileType
    {
        public static bool IsImageVideoMP3ZipFile(IFormFile file, string mimes, out FileType fileType)
        {
            if (file is null)
            {
                fileType = FileType.Unknown;
                return false;
            }

            var typeFromDb = mimes.Split("/");
            if (typeFromDb[0] == "image" || typeFromDb[0] == "audio" || typeFromDb[0] == "video")
            {
                byte[] header = new byte[4];

                using (var fs = file.OpenReadStream())//new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Read the first 4 bytes of the file to check the magic number
                    fs.Read(header, 0, 4);
                }

                switch (typeFromDb[0])
                {
                    case "image":
                        // Check for image file types
                        if (IsImageFile(header))
                        {
                            fileType = FileType.Image;
                            return true;
                        }
                        break;

                    case "audio":
                        // Check for MP3 file type
                        if (IsMp3File(header))
                        {
                            fileType = FileType.MP3;
                            return true;
                        }
                        break;

                    case "video":

                        // Check for video file types
                        if (IsVideoFile(header))
                        {
                            fileType = FileType.Video;
                            return true;
                        }
                        break;
                }
            }
            else
            {
                if (typeFromDb[1] == "pdf")
                {
                    byte[] header = new byte[5];

                    using (var fs = file.OpenReadStream())//new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Read the first 4 bytes of the file to check the magic number
                        fs.Read(header, 0, 5);
                    }
                    // Check for pdf file types
                    if (IsPdfFile(header))
                    {
                        fileType = FileType.Pdf;
                        return true;
                    }
                }
                else
                {
                    byte[] header = new byte[8];

                    using (var fs = file.OpenReadStream())//new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Read the first 4 bytes of the file to check the magic number
                        fs.Read(header, 0, 8);
                    }
                    switch (typeFromDb[1])
                    {

                        case "vnd.openxmlformats-officedocument.wordprocessingml.document":
                            // Check for word file type
                            if (IsWordFile(header))
                            {
                                fileType = FileType.Word;
                                return true;
                            }
                            break;

                        case "vnd.openxmlformats-officedocument.spreadsheetml.sheet":

                            // Check for excel file types
                            if (IsExcelFile(header))
                            {
                                fileType = FileType.Excel;
                                return true;
                            }
                            break;
                    }
                }
            }

            // Check for ZIP/RAR file types
            //if (IsZipRarFile(header))
            //{
            //    fileType = FileType.ZipRar;
            //    return true;
            //}

            //// Check for executable file types
            //if (IsExecutableFile(header))
            //{
            //    fileType = FileType.Executable;
            //    return false;
            //}

            // Unknown file type
            fileType = FileType.Unknown;
            return false;
        }

        private static bool IsImageFile(byte[] header)
        {
            // Add your image file type checks here
            // For example, check for JPEG (0xFF 0xD8 0xFF), PNG (0x89 0x50 0x4E 0x47), etc.
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return true; // JPEG
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
                return true; // PNG
            return false;
        }

        private static bool IsVideoFile(byte[] header)
        {
            // Add your video file type checks here
            // For example, check for MP4 (0x00 0x00 0x00 0x18), AVI (0x52 0x49 0x46 0x46), etc.
            if (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x00 && header[3] == 0x18)
                return true; // MP4
            if (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x00 && header[3] == 0x20)
                return true; // MP4
            if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46)
                return true; // AVI
            return false;
        }

        private static bool IsMp3File(byte[] header)
        {
            // Check for MP3 file type
            if (header[0] == 0x49 && header[1] == 0x44 && header[2] == 0x33)
                return true; // MP3
            return false;
        }

        private static bool IsZipRarFile(byte[] header)
        {
            // Check for ZIP file type
            if (header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04)
                return true; // ZIP
                             // Check for RAR file type
            if (header[0] == 0x52 && header[1] == 0x61 && header[2] == 0x72 && header[3] == 0x21)
                return true; // RAR
            return false;
        }

        private static bool IsExecutableFile(byte[] header)
        {
            // Check for executable file types
            // For example, check for PE (0x4D 0x5A) or ELF (0x7F 0x45 0x4C 0x46) headers
            if (header[0] == 0x4D && header[1] == 0x5A)
                return true; // PE (Windows executable)
            if (header[0] == 0x7F && header[1] == 0x45 && header[2] == 0x4C && header[3] == 0x46)
                return true; // ELF (Linux executable)
            return false;
        }

        private static bool IsPdfFile(byte[] header)
        {
            // Check for MP3 file type
            if (header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46 && header[4] == 0x2D)
                return true; // MP3
            return false;
        }
        private static bool IsWordFile(byte[] header)
        {
            // Check for word file type
            //if (header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0 && header[4] == 0xA1 && header[5] == 0xB1 && header[6] == 0x1A && header[7] == 0xE1)
            if (header[0] == 208 && header[1] == 207 && header[2] == 17 && header[3] == 224 && header[4] == 161 && header[5] == 177 && header[6] == 26 && header[7] == 225)
                return true; // word
            //if (header[0] == 0x50 && header[1] == 0x48 && header[2] == 0x03 && header[3] == 0x04 && header[4] == 0x14 && header[5] == 0x00 && header[6] == 0x00 && header[7] == 0x00)
            if (header[0] == 80 && header[1] == 75 && header[2] == 3 && header[3] == 4 && header[4] == 20 && header[5] == 0 && header[6] == 6 && header[7] == 0)
                return true; // word
            return false;
        }
        private static bool IsExcelFile(byte[] header)
        {
            // Check for excel file type
            if (header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0 && header[4] == 0xA1 && header[5] == 0xB1 && header[6] == 0x1A && header[7] == 0xE1)
                return true; // excel
            if (header[0] == 0x50 && header[1] == 0x48 && header[2] == 0x03 && header[3] == 0x04 && header[4] == 0x14 && header[5] == 0x00 && header[6] == 0x06 && header[7] == 0x00)
                return true;
            return false;
        }
        public enum FileType
        {
            Image,
            Video,
            MP3,
            Pdf,
            Word,
            Excel,
            ZipRar,
            Executable,
            Unknown
        }
    }
}
