using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Graph
{
    public interface IGraph<T>
    {
        IObservable<IEnumerable<T>> RoutesBetween(T source, T target);
    }

    public class Graph<T> : IGraph<T>
    {
        public IEnumerable<ILink<T>> Links;

        public Graph(IEnumerable<ILink<T>> links)
        {
            this.Links = links;
        }

        public IObservable<IEnumerable<T>> RoutesBetween(T source, T target)
        {
            bool startMap = false;
            StringBuilder sbRoutes = new StringBuilder();
            List<string> routesBetween = new List<string>();

            foreach (Link<T> i in this.Links)
            {
                if (i.Source.Equals(source) && startMap == false)
                {
                    
                    sbRoutes.Append(source + "-");
                    sbRoutes.Append(i.Target + "-");
                    startMap = true;
                    continue;
                }

                if (startMap)
                {
                    if (!i.Target.Equals(target))
                    {
                        if (!sbRoutes.ToString().Contains(i.Target.ToString()))
                            sbRoutes.Append(i.Target + "-");
                    }
                    else
                    {
                        sbRoutes.Append(i.Target);
                        routesBetween.Add(sbRoutes.ToString());
                        sbRoutes.Clear();
                        startMap = false;
                        continue;
                    }
                }
            }

            return Observable.Create<IEnumerable<T>>(observer =>
            {
                observer.OnNext((IEnumerable<T>) routesBetween.AsEnumerable());
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
    }
}
