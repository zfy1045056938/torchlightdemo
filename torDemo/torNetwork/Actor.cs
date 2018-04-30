using System;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;
using System.Collections;



namespace torNetwork
{
    /// <summary>
    /// Entity ->Actor
    /// </summary>
    public class Actor
    {

        public int Id;

        public BufferBlock<ActorMsg> mailBox = new BufferBlock<ActorMsg>();

        protected bool isStop = false;

        //Init
        public Actor(){
            
        }


        /// <summary>
        /// Dispath this instance.
        /// </summary>
        /// <returns>The dispath.</returns>
        private async Task Dispath(){
            while (!isStop)
            {
                var msg = await mailBox.ReceiveAsync();
                Console.WriteLine("thread reveive"+this.GetType());
                Console.WriteLine("receive msg"+msg);
                await ReceiveMsg(msg);
            }
        }

        //
        protected static async Task ReceiveMsg(ActorMsg msg){
            await Task.FromResult(default(object));
        }


        //
        protected static void RunTask(System.Func<Task>cb){
            //
            var surroundContext = SynchronizationContext.Current;
            //
            SynchronizationContext.SetSynchronizationContext(_messageQueue);
            //StartNew *
            Task.Factory.StartNew(cb,
                                  CancellationToken.None,
                                  TaskCreationOptions.DenyChildAttach,
                                  TaskScheduler.FromCurrentSynchronizationContext());
            SynchronizationContext.SetSynchronizationContext(surroundContext);
            Console.WriteLine("Task Begin"+surroundContext.ToString());
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        public virtual void Init(){
            RunTask(Dispath);
        }

        public void Stop(){
            isStop = true;
        }
    }

    /// <summary>
    /// Actor util.
    /// </summary>
    public static class ActorUtil{

        public static void SendMsg(this Actor target,string msg){
            var m = new ActorMsg();
            m.msg = msg;
            target.mailBox.SendAsync(m);
        }

        public static void SendMsg(this Actor target, KBEngine.Packet packet)
        {
            var m = new ActorMsg() { packet = packet };
            target.mailBox.SendAsync(m);
        }


        public static void SendMsg(this Actor target, ActorMsg msg)
        {
            target.mailBox.SendAsync(msg);
        }



    }


    /// <summary>
    /// Actor message.
    /// </summary>
    public class ActorMsg{
        public string msg;
        public KBEngine.Packet packet;
        public object obj;
        public object obj1;


    }

    /// <summary>
    /// Actor manager.
    /// </summary>
    public class ActorManager{
        public static ActorManager Instance;

        Dictionary<int, Actor> actorDict;

        Dictionary<Type, Actor> actorType;

        private int actId = 0;

        bool isStop = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:torNetwork.ActorManager"/> class.
        /// </summary>
        public ActorManager(){
            actorDict = new Dictionary<int, Actor>();
            actorType = new Dictionary<Type, Actor>();
            Instance = this;
        }

        public int AddActor(Actor actor,bool addType =false){
            Console.WriteLine("Addctor"+actor+"addType"+addType);
            if (isStop)
            {

                return -1;
            }
            var id = Interlocked.Increment(ref actId);
            lock (actorDict)
            {
                //add actor then lock
                actorDict.Add(id, actor);
                //check exist
                if (addType)
                {
                    actorType.Add(actor.GetType(), actor);
                }
            }

            //
            actor.Id = id;
            actor.Init();
            return id;
        }

        /// <summary>
        /// Check exist then remove actor 
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void RemoveActor(int id){
            lock (actorDict)
            {
                if (actorDict.ContainsKey(id))
                {
                    var act = actorDict[id];
                    if (actorType.ContainsKey(act.GetType()))
                    {
                        var act2 = actorType[act.GetType()];
                        if (act2== act)
                        {
                            actorType.Remove(act.GetType());
                        }
                    }

                }
                actorDict.Remove(id);
            }
        }

        /// <summary>
        /// Gets the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        /// <param name="key">Key.</param>
        public Actor GetActor(int key){
            Actor ret = null;
            lock (actorDict)
            {
                //
                actorDict.TryGetValue(key, out ret);
            }
            return ret;
        }

        /// <summary>
        /// Gets the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetActor<T>() where T: Actor{
            T ret = null;
            lock (actorDict)
            {
                Actor a = null;
                actorType.TryGetValue(typeof(T), out a);
                ret = (T)a;


            }
            return ret;
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop(){
            isStop = true;
            lock (actorDict)
            {
                    foreach (var act in actorDict)
                {
                    act.Value.Stop();

                }
            }
        }
    }
}
