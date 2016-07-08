using System;
using System.Security.Principal;
using System.Web;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using mImO.Contracts.DTOs;


namespace mImO.DataProviders.NHibernate
{
    class NHibernateEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public Guid get_identity()
        {
            Guid identity_of_updater=Guid.NewGuid();


            //string identity_of_updater = WindowsIdentity.GetCurrent().Name;
            //if (HttpContext.Current != null)
            //{
            //    try
            //    {
            //        identity_of_updater = HttpContext.Current.User.Identity.Name;
            //    }
            //    catch
            //    {
            //        //move on
            //    }
            //}
            return identity_of_updater;
        }


        public bool OnPreInsert(PreInsertEvent eventItem)
        {
            mImO.Contracts.DTOs.PersistableBase audit = eventItem.Entity as mImO.Contracts.DTOs.PersistableBase;
            if (audit == null)
            {
                return false;
            }
            DateTime? entered_date = DateTime.Now;
            DateTime? modified_date = DateTime.Now;
            //Guid identity_of_updater = get_identity();
            store(eventItem.Persister, eventItem.State, "CreationDate", entered_date);
            store(eventItem.Persister, eventItem.State, "Ts", modified_date);
            store(eventItem.Persister, eventItem.State, "IdCreador", audit.IdCreador);
            audit.CreationDate = entered_date;
            audit.Ts = modified_date;
            audit.IdCreador = audit.IdCreador;
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent eventItem)
        {
            mImO.Contracts.DTOs.PersistableBase audit = eventItem.Entity as mImO.Contracts.DTOs.PersistableBase;
            if (audit == null)
            {
                return false;
            }
            DateTime? modified_date = DateTime.Now;
            Guid identity_of_updater = get_identity();
            store(eventItem.Persister, eventItem.State, "Ts", modified_date);
            //store(eventItem.Persister, eventItem.State, "IdCreador", identity_of_updater);
            audit.Ts = modified_date;
            //audit.IdCreador = identity_of_updater;
            return false;   
        }

        public void store(IEntityPersister persister, object[] state, string property_name, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, property_name);
            if (index == -1)
            {
                return;
            }
            state[index] = value;
        }
    }
}



