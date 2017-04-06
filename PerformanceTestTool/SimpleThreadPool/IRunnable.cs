// ***********************************************************************
// Assembly         : TSharp.Core
// Author           : tangjingbo
// Created          : 01-07-2014
//
// Last Modified By : tangjingbo
// Last Modified On : 01-16-2014
// ***********************************************************************
// <copyright file="IRunnable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace PerformanceTool.SimpleThreadPool
{
    /// <summary>
    /// This interface should be implemented by any class whose instances are intended
    /// to be executed by a thread.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// This method has to be implemented in order that starting of the thread causes the object's
        /// run method to be called in that separately executing thread.
        /// </summary>
        void Run();
    }
}