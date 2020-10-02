using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RPGMenuNode : UINode
{
    public RPGMenuData MenuData;
    public RPGMenuNode(string menuName) : base()
    {
        MenuData = new RPGMenuData(menuName);
    }
}

