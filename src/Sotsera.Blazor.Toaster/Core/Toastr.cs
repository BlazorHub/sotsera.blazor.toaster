﻿using System;
using System.Collections.Generic;
using Sotsera.Blazor.Toaster.Core.Configuration;
using Sotsera.Blazor.Toaster.Core.Models;

namespace Sotsera.Blazor.Toaster.Core
{
    public class Toaster : IToaster
    {
        public ToasterConfiguration Configuration { get; }
        public event Action OnToastsUpdated;
        public IList<Toast> Toasts { get; private set; } = new List<Toast>();

        public Toaster(ToasterConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Info(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Info, message, title, configure);
        }

        public void Success(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Success, message, title, configure);
        }

        public void Warning(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Warning, message, title, configure);
        }

        public void Error(string message, string title = null, Action<ToastOptions> configure = null)
        {
            Add(ToastType.Error, message, title, configure);
        }

        public void Clear()
        {
            var toasts = Toasts;
            Toasts = new List<Toast>();
            OnToastsUpdated?.Invoke();
            DisposeToasts(toasts);
        }

        public void Remove(Toast toast)
        {
            toast.OnClose -= Remove;
            Toasts.Remove(toast);
            OnToastsUpdated?.Invoke();
            toast.Dispose();
        }

        private void Add(ToastType type, string message, string title, Action<ToastOptions> configure)
        {
            if (string.IsNullOrEmpty(message)) return;

            var options = new ToastOptions(type, Configuration);
            configure?.Invoke(options);

            var toast = new Toast(title, message, options);
            toast.OnClose += Remove;
            Toasts.Add(toast);

            OnToastsUpdated?.Invoke();
        }

        public void Dispose()
        {
            DisposeToasts(Toasts);
        }

        private void DisposeToasts(IEnumerable<Toast> toasts)
        {
            foreach (var toast in toasts)
            {
                toast.OnClose -= Remove;
                toast.Dispose();
            }
        }
    }
}