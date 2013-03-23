using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/* All mailboxes that are read from unity run in non blocking mode, so as not to 
 * block unity's execution.
 * All mailboxes that are read by the agents run in blocking mode, becuase they 
 * are used to synchronize them with unity.
 */

public class MailBox<T> {

    private Queue<T>  queue;
    private Semaphore semaphore;
    private bool      nbmode;

    public MailBox(bool nbmode) {
        this.nbmode = nbmode;
        queue       = new System.Collections.Generic.Queue<T>();
        semaphore   = new Semaphore(0, Int32.MaxValue);
    }

    public void Send(T item) {
        lock (this) {
            queue.Enqueue(item);
            semaphore.Release();
        }            
    }

    /* 
     * The method guarantees that an item will be dequeued, blocking 
     * if the queue is empty until an item is inserted and it can be
     * dequeued.
     * 
     * This Recv method will first block on the semaphore, 
     * which indicates whether there are any items in the queue or not.
     * If there are elements in the queue, the semaphore's value 
     * should be greater than zero, and the caller will attempt to 
     * acquire the mutex.
     * Once the mutex is acquired, it will check to see if the queue 
     * is in fact non-empty, and dequeu an item. 
     * This check is rather redundant if all things go well, 
     * because the caller should never get to the point in which it 
     * will attempt a Recv if the semaphore's value is non-zero.
     * In practice, the call to Count should always return a value 
     * greater than zero, the method always return true, and item
     * always be assigned an element dequeued from the queue. 
     * However, I am loath to remove the check until the code is 
     * more tested in case there is some situation I have not 
     * considered.
     * 
     * The wait must be performed first on the semaphore because if it
     * is done first on the mutex, the caller may acquire the mutex 
     * when the queue is empty. No other caller may acquire the mutex
     * to insert an item in the queue, and a deadlock will occur.  
     */
    public bool Recv(out T item) {
        bool result = false;
        item = default(T);

        if (nbmode) {
            Debug.LogError("PANIC! queue is in non-blocking mode, blocking recv called.");
        }
        else {
            semaphore.WaitOne();
            lock (this) {
                if (queue.Count > 0) {
                    item = (T)queue.Dequeue();
                    result = true;
                } else {
                    Debug.LogError("PANIC! queue is empty.");
                }
            }
        }
        return result;
    }

    public bool NBRecv(out T item) {
        bool result = false;
        item = default(T);

        if (nbmode) {
            lock (this) {
                if (queue.Count > 0) {
                    item = (T)queue.Dequeue();
                    result = true;
                }
            }
        }
        else {
            Debug.LogError("PANIC: queue used in blocking mode, non-blocking recv called.");            
        }
        return result;
    }

    public bool IsEmpty() {
        return (queue.Count == 0);
    }
    
    public bool NotEmpty() {
        return (queue.Count > 0);
    }
}