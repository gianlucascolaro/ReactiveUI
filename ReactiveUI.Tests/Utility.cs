﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using Xunit;

namespace ReactiveUI.Tests
{
    public static class EnumerableTestMixin
    {
        public static void AssertAreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var left = lhs.ToArray();
            var right = rhs.ToArray();

            try {
                Assert.Equal(left.Length, right.Length);
                for (int i = 0; i < left.Length; i++) {
                    Assert.Equal(left[i], right[i]);
                }
            } catch {
                Console.Error.WriteLine("lhs: [{0}]",
                    String.Join(",", lhs.ToArray()));
                Console.Error.WriteLine("rhs: [{0}]",
                    String.Join(",", rhs.ToArray()));
                throw;
            }
        }
    }

    public class CountingTestScheduler : IScheduler
    {
        public CountingTestScheduler(IScheduler innerScheduler)
        {
            InnerScheduler = innerScheduler;
            ScheduledItems = new List<Tuple<Action, TimeSpan?>>();
        }

        public IScheduler InnerScheduler { get; private set; }
        public List<Tuple<Action, TimeSpan?>> ScheduledItems { get; private set; }

        public IDisposable Schedule(Action action)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(action, null));
            return InnerScheduler.Schedule(action);
        }

        public IDisposable Schedule(Action action, TimeSpan dueTime)
        {
            ScheduledItems.Add(new Tuple<Action, TimeSpan?>(action, dueTime));
            return InnerScheduler.Schedule(action, dueTime);
        }

        public DateTimeOffset Now {
            get { return InnerScheduler.Now; }
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action, DateTimeOffset dueTime)
        {
            throw new NotImplementedException();
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action, TimeSpan dueTime)
        {
            throw new NotImplementedException();
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotImplementedException();
        }
    }

    internal static class CompatMixins
    {
        public static void Run<T>(this IEnumerable<T> This, Action<T> block)
        {
            foreach (var v in This) {
                block(v); 
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> This, int count)
        {
            return This.Take(This.Count() - count);
        }
    }
}