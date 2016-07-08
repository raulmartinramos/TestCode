using System;
using System.Security.Principal;
using System.Web;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using Essence.Dto;



namespace Essence.DataProviders.NHibernate
{
    class NHibernateEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public Guid get_identity()
        {
            Guid identity_of_updater=Guid.NewGuid();
            return identity_of_updater;
        }


        public bool OnPreInsert(PreInsertEvent eventItem)
        {
            PersistableBase audit = eventItem.Entity as PersistableBase;
            if (audit == null)
            {
                return false;
            }
            DateTime? entered_date = DateTime.Now;
            DateTime? modified_date = DateTime.Now;
            //Guid identity_of_updater = get_identity();
            store(eventItem.Persister, eventItem.State, "CreationDate", entered_date);
            store(eventItem.Persister, eventItem.State, "Ts", modified_date);
            audit.CreationDate = entered_date;
            audit.Ts = modified_date;
            return false;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent eventItem)
        {
            PersistableBase audit = eventItem.Entity as PersistableBase;
            if (audit == null)
            {
                return false;
            }
            DateTime? modified_date = DateTime.Now;
            store(eventItem.Persister, eventItem.State, "Ts", modified_date);
            audit.Ts = modified_date;
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



