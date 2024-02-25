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

        private Dictionary<string, ClassEntity> _classEntities = new Dictionary<string, ClassEntity>();
        public IReadOnlyDictionary<string, ClassEntity> classEntities => _classEntities.AsReadOnly();
        public ClassEntity? GetExistingClassEntity(string belongingNamepsace, string identifier)
        {
            throw new NotImplementedException();
        }

        public void AddClassEntityInstance(ClassEntity entity)
        {
            _classEntities.Add(entity.name, entity);
        }
    }
}
