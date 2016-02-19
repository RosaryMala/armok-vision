using System.Collections.Generic;

namespace MapGen
{
    public class UpdateSchedule
    {
        private readonly List<int> ticks = new List<int>();
        private int lastUpdateTick;
        private int nextUpdateIndex;

        public void Add(int tick)
        {
            if (tick < 0)
                return;
            if (!ticks.Contains(tick))
            {
                ticks.Add(tick);
                ticks.Sort();

                nextUpdateIndex = 0;
            }
        }

        public void Clear()
        {
            ticks.Clear();
            nextUpdateIndex = 0;
        }

        public bool CheckUpdate(int tick)
        {
            if (tick < lastUpdateTick)
            {
                nextUpdateIndex = 0;
            }

            lastUpdateTick = tick;

            if (nextUpdateIndex >= ticks.Count)
            {
                return false;
            }

            var result = false;

            while (nextUpdateIndex < ticks.Count && ticks[nextUpdateIndex] < tick)
            {
                result = true;

                nextUpdateIndex++;
            }

            return result;
        }
    }
}