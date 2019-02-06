using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using DAL;
using Interfaces;

namespace BLL
{
    public class EstadoBLL : IEstado
    {

        /// <summary>
        /// Consulta todos os Estados, ou um especifico
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<EstadoDTO> Consultar(int Id)
        {
            using (EstadoDAL DAL = new EstadoDAL(false))
            {
                return DAL.Consultar(Id);
            }
        }
    }
}
