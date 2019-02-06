using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class EstadoDTO
    {

        #region Privado

        private int _Id;
        private string _Descricao;

        #endregion

        #region Publico

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        public string Descricao
        {
            get
            {
                return _Descricao;
            }
            set
            {

                _Descricao = value;
            }
        }

        #endregion

    }
}
