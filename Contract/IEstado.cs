using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;

namespace Interfaces
{
    public interface IEstado
    {

        List<EstadoDTO> Consultar(int Id);

    }
}
