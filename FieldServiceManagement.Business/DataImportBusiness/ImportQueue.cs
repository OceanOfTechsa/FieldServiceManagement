using FieldServiceManagement.ViewModels.Contact;
using System.Threading.Channels;

namespace FieldServiceManagement.Business.Imports
{
    public static class ImportQueue
    {
        public static readonly Channel<ContactImportJob> Jobs = Channel.CreateUnbounded<ContactImportJob>();
    }
}