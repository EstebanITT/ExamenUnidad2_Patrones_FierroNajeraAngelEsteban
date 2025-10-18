using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense
{
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> objetos = new Stack<T>();

        public ObjectPool(IEnumerable<T> objetosIniciales)
        {
            foreach (var obj in objetosIniciales)
                objetos.Push(obj);
        }

        public T ObtenerObjeto()
        {
            return objetos.Count > 0 ? objetos.Pop() : null; // No crear nuevos
        }

        public void DevolverObjeto(T obj)
        {
            objetos.Push(obj);
        }

        public int CantidadDisponible()
        {
            return objetos.Count;
        }
    }
}
