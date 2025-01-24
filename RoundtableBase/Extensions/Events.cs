using SoulsFormats;

namespace RoundtableBase.Extensions;

public static class Events
{
    /// <summary>
    /// Retrieve an Event in `EMEVD` by its event ID, or create and return a new empty Event if it doesn't exist.
    /// </summary>
    /// <param name="emevd"></param>
    /// <param name="eventID"></param>
    /// <returns></returns>
    public static EMEVD.Event FindOrCreateEventID(this EMEVD emevd, long eventID)
    {
        EMEVD.Event? ev = emevd.Events.Find(e => e.ID == eventID);
        if (ev != null)
            return ev;
        
        ev = new EMEVD.Event(eventID);
        emevd.Events.Add(ev);
        return ev;
    }    
}