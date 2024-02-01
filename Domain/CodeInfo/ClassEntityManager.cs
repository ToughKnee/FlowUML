using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CodeInfo
{
    public class ClassEntityManager
    {
        private static ClassEntityManager _instance;

        private ClassEntityManager()
        {
        }

        public static ClassEntityManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClassEntityManager();
                }
                return _instance;
            }
        }

        private List<ClassEntity> _classEntities = new List<ClassEntity>();
        public IReadOnlyCollection<ClassEntity> classEntities => _classEntities.AsReadOnly();
        public ClassEntity GetExistingOrCreateClassEntity(string belongingNamepsace, string identifier)
        {
            throw new NotImplementedException();
        }

        public void ReceiveNewClassEntityInstance(ClassEntity entity)
        {
            _classEntities.Add(entity);
        }
    }
}
