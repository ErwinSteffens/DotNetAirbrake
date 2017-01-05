using System;
using System.Collections.Generic;

namespace DotNetAirbrake
{
    public abstract class ObjectBuilder<T>
        where T : class
    {
        private readonly List<Action<T>> modifiers = new List<Action<T>>();

        public T Build()
        {
            var result = this.OnBuild();

            foreach (var modifier in this.modifiers)
            {
                modifier(result);
            }

            return result;
        }

        public ObjectBuilder<T> With(Action<T> modifier)
        {
            this.modifiers.Add(modifier);
            return this;
        }

        protected virtual T OnBuild()
        {
            return Activator.CreateInstance<T>();
        }
    }
}