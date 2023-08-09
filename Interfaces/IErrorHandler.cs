using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.Interfaces {
    public interface IErrorHandler {
        string? GetLastError();
    }
}
