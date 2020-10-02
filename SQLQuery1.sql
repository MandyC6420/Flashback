Select m.MeetingId, m.UserId  FROM Meeting m
Left Join MeetingAttendant ma
On m.MeetingId = ma.MeetingId;
