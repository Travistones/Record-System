using Record_System.BackEnd.Constants;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.FirebaseStorage
{
    public class FirebaseStorageViewModel : BindableBase
    {
        Firebase.Storage.FirebaseStorage firebaseStorage;

        public FirebaseStorageViewModel()
        {
            init();
        }
        
        private void init()
        {
            firebaseStorage = new(ApplicationConstants.FIREBASE_STORAGE_BUCKET);
        }

        private async Task<string> addImage(string imagePath, string imageName)
        {
            imagePath = imagePath.Replace("%20", " ");

            var fileStream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            var uploadTask = firebaseStorage.Child(ApplicationConstants.IMAGES_FOLDER_NAME).Child(imageName.Replace(" ", string.Empty)).PutAsync(fileStream);

            var downloadUrl = await uploadTask;

            return downloadUrl;
        }

        public async Task<string> AddImage(string imagePath, string imageName) => await addImage(imagePath, imageName);

    }
}
