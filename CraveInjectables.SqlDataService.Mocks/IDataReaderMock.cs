using System.Collections.Generic;

namespace CraveInjectables.SqlDataService.Mocks
{
    public interface IDataReaderMock<out TObject>
    {
        IEnumerator<TObject> Enumerator { get; }
        bool IsClosed { get; }
    }
}