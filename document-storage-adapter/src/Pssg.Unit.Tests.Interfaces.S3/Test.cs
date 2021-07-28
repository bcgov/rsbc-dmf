

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DocumentStorageAdapter;
using Rsbc.Dmf.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace Rsbc.Unit.Tests.Interfaces
{
    public class Test
    {

        IConfiguration Configuration;

        S3 s3;

        string serverAppIdUri;

        /// <summary>
        /// Setup the test
        /// </summary>        
        public Test()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for the document storage adapter.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            s3 = new S3(Configuration);

        }


        [Fact]
        public async void UploadRemoveFilesTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string name = "test-name" + rnd.Next() + ".txt";
            string testFolder = "test-folder" + rnd.Next();
            string listTitle = "Shared Documents";

            string contentType = "text/plain";

            string testData = "This is just a test.";

            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            await s3.CreateFolder(listTitle, testFolder);

            await s3.UploadFile(name, "Shared Documents", testFolder, fileData, contentType);

            // now delete it.

            await s3.DeleteFile("Shared Documents", testFolder, name);

            // cleanup the test folder.
            await s3.DeleteFolder("Shared Documents", testFolder);
        }

        [Fact]
        public async void FolderNameTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string name = "test-name" + rnd.Next() + ".txt";
            string testFolder = "O'Test " + rnd.Next();
            string listTitle = "Shared Documents";

            string contentType = "text/plain";

            string testData = "This is just a test.";

            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            await s3.CreateFolder(listTitle, testFolder);

            await s3.UploadFile(name, "Shared Documents", testFolder, fileData, contentType);

            // now delete it.

            await s3.DeleteFile("Shared Documents", testFolder, name);

            // cleanup the test folder.
            await s3.DeleteFolder("Shared Documents", testFolder);
        }

        [Fact]
        public async void AddRemoveListFilesTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";
            string fileName = documentType + "__" + "test-file-name" + rnd.Next() + ".txt";
            string folderName = "test-folder-name" + rnd.Next();
            string path = "/";
    
            path += S3.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await s3.AddFile(S3.DefaultDocumentListTitle, folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<S3.FileDetailsList> fileDetailsList = await s3.GetFileDetailsListInFolder(S3.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            string serverRelativeUrl = null;
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
                serverRelativeUrl = fileDetails.ServerRelativeUrl;
            }

            // verify that we can download the same file.

            byte[] data = await s3.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await s3.DeleteFile(S3.DefaultDocumentUrlTitle, folderName, fileName);

            // delete folder from SP

            await s3.DeleteFolder(S3.DefaultDocumentUrlTitle, folderName);
        }


        [Fact]
        public async void LongFilenameTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";

            int maxLen = 255 - 4; // Windows allows for a maximum of 255 characters for a given file; subtract 4 for the extension.

            string fileName = documentType + "__" + "test-file-name";
            maxLen -= fileName.Length;
            for (int i = 0; i < maxLen; i++)
            {
                string r = rnd.Next().ToString();
                fileName += r[0];
            }

            fileName += ".txt";

            string folderName = "test-folder-name" + rnd.Next();
            string path = "/";
            if (!string.IsNullOrEmpty(s3.WebName))
            {
                path += $"{s3.WebName}/";
            }

            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            fileName = await s3.AddFile(folderName, fileName, fileData, contentType);

            path += S3.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;

            // get file details list in SP folder

            List<S3.FileDetailsList> fileDetailsList = await s3.GetFileDetailsListInFolder(S3.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            string serverRelativeUrl = null;
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
                serverRelativeUrl = fileDetails.ServerRelativeUrl;
            }

            // verify that we can download the same file.

            byte[] data = await s3.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await s3.DeleteFile(S3.DefaultDocumentUrlTitle, folderName, fileName);

            // delete folder from SP

            await s3.DeleteFolder(S3.DefaultDocumentUrlTitle, folderName);
        }


        [Fact]
        public async void FileNameWithApostropheTest()
        {
            // set file and folder settings

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentType = "Document Type";
            string fileName = documentType + "__" + "test-'-name" + rnd.Next() + ".txt";
            string folderName = "test-folder-name" + rnd.Next();
            string path =  "/" + S3.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string url = serverAppIdUri + s3.WebName + "/" + S3.DefaultDocumentListTitle + "/" + folderName + "/" + fileName;
            string contentType = "text/plain";
            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await s3.AddFile(folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<S3.FileDetailsList> fileDetailsList = await s3.GetFileDetailsListInFolder(S3.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            foreach (S3.FileDetailsList fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
            }

            // verify that we can download the same file.

            byte[] data = await s3.DownloadFile(path);
            string stringData = System.Text.Encoding.ASCII.GetString(data);
            Assert.Equal(stringData, testData);

            // delete file from SP

            await s3.DeleteFile(S3.DefaultDocumentListTitle, folderName, fileName);

            // delete folder from SP

            await s3.DeleteFolder(S3.DefaultDocumentListTitle, folderName);
        }


        /// <summary>
        /// Test Create Folder
        /// </summary>
        [Fact]
        public async void InvalidFolderDoesNotExist()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string documentLocation = "Account";
            string folderName = "Test Folder" + rnd.Next() + "---" + rnd.Next();


            bool exists = await s3.FolderExists(documentLocation, folderName);

            Assert.False(exists);

        }

        /// <summary>
        /// Test Create Folder
        /// </summary>
        [Fact]
        public async void CreateFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test-Folder-" + rnd.Next();

            await s3.CreateFolder(S3.DefaultDocumentUrlTitle, folderName);


            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            string fileName = "TestFile.txt";
            string contentType = "text/plain";

            await s3.AddFile(S3.DefaultDocumentUrlTitle, folderName, fileName, fileData, contentType);

            bool exists = await s3.FolderExists(S3.DefaultDocumentUrlTitle, folderName);

            Assert.True(exists);

            await s3.DeleteFolder(S3.DefaultDocumentUrlTitle, folderName);

            exists = await s3.FolderExists(S3.DefaultDocumentUrlTitle, folderName);

            Assert.False(exists);
        }

        [Fact]
        public async void GetFilesInEmptyFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test Folder" + rnd.Next();
            string documentType = "Corporate Information";
            await s3.CreateFolder(S3.DefaultDocumentListTitle, folderName);

            var files = await s3.GetFileDetailsListInFolder(S3.DefaultDocumentListTitle, folderName, documentType);
            Assert.True(files != null);
            Assert.True(files.Count == 0);
            await s3.DeleteFolder(S3.DefaultDocumentListTitle, folderName);
        }

        [Fact]
        public async void GetFilesInPopulatedFolderTest()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string folderName = "Test Folder" + rnd.Next();
            string documentType = "Corporate Information";
            await s3.CreateFolder(S3.DefaultDocumentListTitle, folderName);

            string fileName = documentType + "__" + "test-file-name" + rnd.Next() + ".txt";
            string contentType = "text/plain";

            string testData = "This is just a test.";
            MemoryStream fileData = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(testData));

            // add file to SP

            await s3.AddFile(folderName, fileName, fileData, contentType);

            // get file details list in SP folder

            List<S3.FileDetailsList> fileDetailsList = await s3.GetFileDetailsListInFolder(S3.DefaultDocumentListTitle, folderName, documentType);
            //only one file should be returned
            Assert.Single(fileDetailsList);
            // validate that file name uploaded and listed are the same
            foreach (var fileDetails in fileDetailsList)
            {
                Assert.Equal(fileName, fileDetails.Name);
            }

            // delete file from SP

            await s3.DeleteFile(S3.DefaultDocumentListTitle, folderName, fileName);


            await s3.DeleteFolder(S3.DefaultDocumentListTitle, folderName);
        }

    }
}
