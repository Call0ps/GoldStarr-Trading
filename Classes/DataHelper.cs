using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;


namespace GoldStarr_Trading
{

    public class DataHelper
    {

        #region Properties

        /// <summary>
        /// The file from which the program will read and write to.
        /// </summary>
        private string _fileName { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor that is fired instantly when an object is being initiated.
        /// </summary>
        /// <param name="fileName">The filename to read from and write to.</param>
        public DataHelper(string fileName)
        {
            _fileName = fileName;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Reads from a JSON file by filename and returns the result T.
        /// </summary>
        /// <typeparam name="T">The type of object that is being stored in the file</typeparam>
        /// <returns>The object T</returns>
        public async Task<T> ReadFromFile<T>()
        {
            // Locate the folder which this UWP application can read from and write to.
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Initiate a file variable.
            Windows.Storage.StorageFile file;
            try
            {
                // if the file exists, the file variable will be set to that.
                file = await storageFolder.GetFileAsync(_fileName);
            }
            catch (FileNotFoundException fnfe)


            {
                // if the file does not exist, an exception will be thrown, but we will make sure the file will be created.
                file = await storageFolder.CreateFileAsync(_fileName);
            }
            catch (Exception ex)
            {
                // if this exception occurs, this means we do not have handled the an exception and we want the program to crash.
                throw ex;
            }

            var text = await Windows.Storage.FileIO.ReadTextAsync(file);
            T obj = JsonConvert.DeserializeObject<T>(text);

            return obj;
        }
        /// <summary>
        /// Writes T to a file by the initial filename as JSON.
        /// </summary>
        /// <typeparam name="T">The object type to store</typeparam>
        /// <param name="data">The actual object to store in the file</param>
        public async Task<bool> WriteToFile<T>(T data)
        {
            // Convert data to JSON object (https://www.w3schools.com/whatis/whatis_json.asp)
            string jsonContent = JsonConvert.SerializeObject(data, Formatting.None);

            // Locate the folder which this UWP application can read from and write to.
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Initiate a file variable.
            Windows.Storage.StorageFile file;
            try
            {
                // if the file exists, the file variable will be set to that.
                file = await storageFolder.GetFileAsync(_fileName);
            }
            catch (FileNotFoundException fnfe)
            {
                // if the file does not exist, an exception will be thrown, but we will make sure the file will be created.
                file = await storageFolder.CreateFileAsync(_fileName);
            }
            catch (Exception ex)
            {
                // if this exception occurs, this means we do not have handled the an exception and we want the program to crash.
                throw ex;
            }

            try
            {
                // Now, write the JSON object to the actual file.
                await Windows.Storage.FileIO.WriteTextAsync(file, jsonContent, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }

}