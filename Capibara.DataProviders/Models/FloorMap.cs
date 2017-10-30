using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Capibara.Net.Rooms;

namespace Capibara.Models
{
    /// <summary>
    /// フロアマップモデル
    /// </summary>
    public class FloorMap : ModelBase<FloorMap>
    {
        public event EventHandler RefreshSuccess;
        
        public event EventHandler<Exception> RefreshFail;

        public ObservableCollection<Room> Rooms { get; }
            = new ObservableCollection<Room>();

        public async Task Refresh()
        {
            var request = new IndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                
                this.Rooms.Clear();

                response.Rooms?.ForEach(x => this.Rooms.Add(x));

                this.RefreshSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.RefreshFail?.Invoke(this, e);
            }
        }
    }
}
