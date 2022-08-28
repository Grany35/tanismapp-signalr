using Business.Abstract;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Utilities.Cloudinary;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class PhotoManager : IPhotoService
    {
        public IConfiguration Configuration { get; }
        private CloudinarySettings _cloudinarySettings;
        private readonly Cloudinary _cloudinary;
        private readonly IPhotoDal _photoDal;


        public PhotoManager(IPhotoDal photoDal, IConfiguration configuration)
        {
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var acc = new Account
                (
                    _cloudinarySettings.CloudName,
                    _cloudinarySettings.ApiKey,
                    _cloudinarySettings.ApiSecret
                );
            _cloudinary = new Cloudinary(acc);
            _photoDal = photoDal;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {

            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        public async Task AddPhoto(Photo photo)
        {
            await _photoDal.Add(photo);
        }

        public IDataResult<Photo> GetPhoto(int photoId)
        {
            return new SuccessDataResult<Photo>(_photoDal.Get(x => x.Id == photoId));
        }

        public void Update(Photo photo)
        {
            _photoDal.Update(photo);
        }
    }
}
