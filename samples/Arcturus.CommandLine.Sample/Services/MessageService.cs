using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arcturus.CommandLine.Sample.Services;

public class MessageService
{
    public async Task<string> GetMessageAsync()
    {
        await Task.Delay(100);
        return "Hello from MessageService!";
    }
}
