using Datatec.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datatec.Implementation
{
    public class Tarea
    {
        public Timer _timer;
        private Action _callback;
        private Periodo _periodo;
        public Periodo Periodo { get { return _periodo; } }
        public bool Running { get; private set; }

        public Tarea(Action callback, Periodo periodo)
        {
            _callback = callback;
            _periodo = periodo;
        }
        public void Iniciar()
        {
            TimeSpan inicio = TimeSpan.Zero;
          
            if (_periodo.HoraInicio > DateTime.Now)
             inicio = _periodo.HoraInicio.Subtract(DateTime.Now);

            if (DateTime.Now < _periodo.HoraFin )
            {
                _timer = new Timer((e) =>
                {
                    Running = true;

                    if (Between(DateTime.Now, _periodo.HoraInicio, _periodo.HoraFin))
                        _callback();

                    if (DateTime.Now > _periodo.HoraFin)
                        Detener();

                },null,inicio, _periodo.IntervaloRevision);

            }
            
        }

        private bool Between(DateTime input, DateTime date1, DateTime date2)
        {
            return (input > date1 && input < date2);
        }

        public void Detener()
        {
            if(this.Running)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                Running = false;
            }
            
            
        }

    }
}
