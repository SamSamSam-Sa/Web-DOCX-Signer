using System;
using System.Collections.Generic;

namespace web_docx_signer.Services
{
    public interface ITempDataService
    {
        void Add(Guid fileGuid, byte[] body);
        byte[] Get(Guid fileGuid);
        bool IsExist(Guid fileGuid);
    }

    public class TempDataService: ITempDataService
    {
        private readonly Dictionary<Guid, FileModel> _tempData;

        public TempDataService()
        {
            _tempData = new Dictionary<Guid, FileModel>();
        }

        public void Add(Guid fileGuid, byte[] body)
        {
            _tempData[fileGuid] = new FileModel(){ Body = body, Age = DateTime.Now };
        }

        public byte[] Get(Guid fileGuid)
        {
            var fileModel = _tempData[fileGuid];
            _tempData.Remove(fileGuid);
            return fileModel.Body;
        }

        public bool IsExist(Guid fileGuid)
        {
            return _tempData.ContainsKey(fileGuid);
        }
    }
}
