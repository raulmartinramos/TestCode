using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using NHibernate.Engine;
using NHibernate.Id;
using Essence.Dto;


namespace Essence.DataProviders.NHibernate.Maps
{
    public abstract class PersistenceMapping<TDomainEntity> :
        ClassMap<TDomainEntity> where TDomainEntity : PersistableBase
    {
        public PersistenceMapping()
        {
            //Id(x => x.Id).GeneratedBy.GuidComb().UnsavedValue(Guid.Empty);
            Id(x => x.Id).GeneratedBy.Custom(typeof(GuidStringGenerator)).UnsavedValue(Guid.Empty);
            OptimisticLock.Version();
            Version(x => x.Version).Column("Version").Generated.Always();
            Map(x => x.Ts);
            Map(x => x.CreationDate);
        }
    }


    public class GuidStringGenerator : IIdentifierGenerator
    {
        public object Generate(ISessionImplementor session, object obj)
        {
            if (((PersistableBase)obj).Id == Guid.Empty)
                return new GuidCombGenerator().Generate(session, obj);
            else
                return ((PersistableBase)obj).Id;
        }
    }

}
