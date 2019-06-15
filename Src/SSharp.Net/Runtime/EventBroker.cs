/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Scripting.SSharp.Runtime
{
  /// <summary>
  /// Manages event subscriptions from the script
  /// </summary>
  [Promote(false)]
  public abstract class EventBroker
  {
    protected IInvokable Target { get; set; }

    private static Dictionary<object, Dictionary<EventInfo, List<InvocationInfo>>> Subscriptions = new Dictionary<object, Dictionary<EventInfo, List<InvocationInfo>>>();
    private static readonly Dictionary<IInvokable, Script> MethodContextMapping = new Dictionary<IInvokable, Script>();

    internal static void RegisterFunction(IInvokable function, Script script)
    {
      MethodContextMapping.Add(function, script);
    }

    internal static void AssignEvent(EventInfo ei, object targetObject, IInvokable function)
    {
      var eventHelper = typeof(EventHelper<>);
      var actualHelper = eventHelper.MakeGenericType(
        ei.GetAddMethod().GetParameters()[0].ParameterType.GetMethod("Invoke").GetParameters()[1].ParameterType);
      var eventHandler = actualHelper.GetMethod("EventHandler");
      var target = (EventBroker)Activator.CreateInstance(actualHelper);
      target.Target = function;
      var deleg = Delegate.CreateDelegate(ei.EventHandlerType, target, eventHandler);
      ei.AddEventHandler(targetObject, deleg);
      AddInvokation(targetObject, ei, deleg, function);
    }

    internal static void RemoveEvent(EventInfo ei, object targetObject, IInvokable function)
    {
      Dictionary<EventInfo, List<InvocationInfo>> targetSubscriptions;
      if (Subscriptions.TryGetValue(targetObject, out targetSubscriptions))
      {
          List<InvocationInfo> eventSubscriptions;
          if (targetSubscriptions.TryGetValue(ei, out eventSubscriptions)) {

              foreach (InvocationInfo invocation_info in eventSubscriptions) {
                  if (invocation_info.HandlerFunction != function) 
                      continue;
                  
                  ei.RemoveEventHandler(targetObject, invocation_info.HandlerDelegate);
                  eventSubscriptions.Remove(invocation_info);

                  if (eventSubscriptions.Count == 0)
                      targetSubscriptions.Remove(ei);

                  break;
              }
          }

          if (eventSubscriptions.Count == 0)
              Subscriptions.Remove(targetObject);
      }
    }

    internal static void ClearMapping(Script script)
    {
      var toRemove = MethodContextMapping.Keys.Where(handler => MethodContextMapping[handler] == script).ToList();

      foreach (var handler in toRemove)
        MethodContextMapping.Remove(handler);
    }

    public static void ClearAllSubscriptions()
    {
      MethodContextMapping.Clear();

      if (Subscriptions == null) return;
      foreach (object targetObject in Subscriptions.Keys.ToArray())
      {
        Dictionary<EventInfo, List<InvocationInfo>> targetInvocations=Subscriptions[targetObject];
        EventInfo[] eventInvocations = targetInvocations.Keys.ToArray();
        foreach (var ei in eventInvocations) {
            InvocationInfo[] invocations = targetInvocations[ei].ToArray();
            foreach (InvocationInfo invocation in invocations)
                RemoveEvent(ei, targetObject, invocation.HandlerFunction);
        }
      }
      Subscriptions.Clear();
    }

    public static bool HasSubscriptions
    {
      get
      {
        return Subscriptions.Count > 0;
      }
    }

    protected static IScriptContext GetContext(IInvokable target)
    {
      try
      {
        return MethodContextMapping[target].Context;
      }
      catch
      {
        throw new ScriptEventException(Strings.ContextNotFoundExceptionMessage);
      }
    }

    private static void AddInvokation(object target, EventInfo ei, Delegate deleg, IInvokable function)
    {
        Dictionary<EventInfo, List<InvocationInfo>> targetSubscriptions;

        if (!Subscriptions.TryGetValue(target, out targetSubscriptions)) {
            targetSubscriptions=new Dictionary<EventInfo, List<InvocationInfo>>();
            Subscriptions.Add(target,targetSubscriptions);
        }

        List<InvocationInfo> eventSubscriptions;
        if (!targetSubscriptions.TryGetValue(ei, out eventSubscriptions)) {
            eventSubscriptions = new List<InvocationInfo>();
            targetSubscriptions.Add(ei, eventSubscriptions);
        }

        eventSubscriptions.Add(new InvocationInfo(deleg, function));
    }
  }

  internal class EventOperatorHandler : IOperatorHandler
  {
    private readonly bool _subscribe;

    public EventOperatorHandler(bool subscribe)
    {
      _subscribe = subscribe;
    }

    #region IOperatorHandler Members

    public object Process(HandleOperatorArgs args)
    {
      if (args == null) throw new NotSupportedException();

      var @event = args.Arguments.FirstOrDefault() as EventInfo;
      if (@event == null) return null;

      args.Cancel = true;
      return _subscribe ? args.Arguments[1] : new RemoveDelegate((IInvokable)args.Arguments[1]);
    }

    #endregion
  }

  [ComVisible(true)]
  internal class EventHelper<T> : EventBroker
  {
    public void EventHandler(object sender, T e)
    {
      if (Target == null) return;

      IScriptContext context = GetContext(Target);
      context.CreateScope(RuntimeHost.ScopeFactory.Create(ScopeTypes.Event, context.Scope, context, Target));

      Target.Invoke(context, new[] { sender, e });
    }
  }

  internal class RemoveDelegate : IInvokable
  {
    public IInvokable OriginalMethod
    {
      get;
      private set;
    }

    public RemoveDelegate(IInvokable original)
    {
      OriginalMethod = original;
    }

    #region IInvokable Members

    public bool CanInvoke()
    {
      return false;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

  internal class InvocationInfo
  {
    public Delegate HandlerDelegate { get; private set; }
    public IInvokable HandlerFunction { get; private set; }

    public InvocationInfo(Delegate handler, IInvokable function)
    {
      HandlerDelegate = handler;
      HandlerFunction = function;
    }
  }
}
