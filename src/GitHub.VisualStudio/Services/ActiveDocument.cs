﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace GitHub.VisualStudio
{
    [Export(typeof(IActiveDocument))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class ActiveDocument : IActiveDocument
    {
        public string Name { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        [ImportingConstructor]
        public ActiveDocument([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Line = Column = -1;
            Name = Services.Dte2?.ActiveDocument?.FullName;
            var textManager = serviceProvider.GetService(typeof(SVsTextManager)) as IVsTextManager;
            Debug.Assert(textManager != null, "No SVsTextManager service available");
            if (textManager == null)
                return;
            IVsTextView view;
            int line, col;
            if (ErrorHandler.Succeeded(textManager.GetActiveView(0, null, out view)) &&
                ErrorHandler.Succeeded(view.GetCaretPos(out line, out col)))
            {
                Line = line;
                Column = col;
            }
        }
    }
}