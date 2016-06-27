using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotCore.Components
{
    public class TargetFinder : UpdateableComponent
    {
        public EventHandler<MapObject[]> OnTargetUpdated  = delegate { };

        private MapObject[] _mobjects = new MapObject[0];

        private MapObject m_SelectedTarget;
        protected MapObject SelectedTarget
        {
            get { return m_SelectedTarget; }
        }

        public MapObject NearestTarget
        {
            get { return SelectedTarget; }
        }

        private List<MapObject> m_TargetedMonsters = new List<MapObject>();

        public List<MapObject> TargetedMonsters
        {
            get
            {
                List<MapObject> copy;
                lock (m_TargetedMonsters)
                {
                    copy = new List<MapObject>(m_TargetedMonsters);
                }
                return copy;
            }
        }

        public List<MapObject> RetrieveMonsterTargets(Predicate<MapObject> predicate)
        {
            return TargetedMonsters.Where(i => i.Type == MapObjectType.Monster)
                .ToList().FindAll(predicate);
        }

        public MapObject RetrieveMonsterTarget(Predicate<MapObject> predicate)
        {
            return TargetedMonsters.Where(i => i.Type == MapObjectType.Monster)
                .ToList().Find(predicate);
        }

        public MapObject RetreivePlayerTarget(Predicate<MapObject> predicate)
        {
            return VisibleObjects.ToList().Find(predicate);
        }

        public List<MapObject> RetreivePlayerTargets(Predicate<MapObject> predicate)
        {
            return VisibleObjects.ToList().FindAll(predicate);
        }

        public MapObject[] VisibleObjects
        {
            get
            {
                List<MapObject> copy;
                lock (_mobjects)
                {
                    copy = new List<MapObject>(_mobjects);
                }
                return copy.ToArray();
            }
        }

        public TargetFinder()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(0.1));
            OnTargetUpdated += TargetsUpdated;
        }

        private void TargetsUpdated(object sender, MapObject[] e)
        {
            SelectTarget(e);
        }

        private void SelectTarget(MapObject[] e)
        {
            m_TargetedMonsters.Clear();
            foreach (var obj in e.OrderBy(i => i.TargetPriority))
            {
                if (obj.CanTarget != null && obj.CanTarget(obj) && obj.Type == MapObjectType.Monster)
                    m_TargetedMonsters.Add(obj);
            }

            m_SelectedTarget = m_TargetedMonsters.FirstOrDefault();
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                OnUpdate();
                Timer.Reset();
            }
        }

        public bool IsUpdating = false;

        private void OnUpdate()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;

                //this is preliminary checks, to ensure target is valid.
                var objects = (from v in Client.FieldMap.MapObjects
                               where v != null && v.Serial != Client.Attributes.Serial
                               && v.ServerPosition.DistanceFrom(Client.Attributes.ClientPosition) < 12
                               orderby v.ServerPosition.DistanceFrom(Client.Attributes.ClientPosition) descending 
                               select v).ToArray();

                if (objects.Length > 0)
                {
                    Array.Resize(ref _mobjects, objects.Length);
                    Array.Copy(objects, 0, _mobjects, 0, objects.Length);

                    OnTargetUpdated(Client, _mobjects);
                }

                IsUpdating = false;
            }
        }
    }
}
