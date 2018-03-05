using System;
using System.Linq;
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

                //this.Rooms.Clear();
                this.Rooms.Any(y => y.Id == response.Rooms[0].Id);
                response.Rooms?.ForEach(x => {
                    if (this.Rooms.Any(y => y.Id == x.Id))
                    {
                        this.Rooms.First(y => y.Id == x.Id).Restore(x);
                    }
                    else
                    {
                        this.Rooms.Add(x);
                    }
                });

                this.RefreshSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                this.RefreshFail?.Invoke(this, e);
            }
        }
    }
}
