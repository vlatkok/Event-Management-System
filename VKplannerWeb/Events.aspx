<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Events.aspx.cs" Inherits="Events" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    

    
    
   <style type="text/css">
      html { height: 100% }
      body { height: 100%; margin: 0; padding: 0 }
      #map-canvas { height: 100% }
      
       .TextandBackGroundStyle
        {
           color:White;
            background-color: #1E90FF;
        }
    </style>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <script type="text/javascript"
     src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBddfWhf0SM6Y_iLBP9IxAS9sYb_7D5IUM&sensor=false">
    </script>
    <script type="text/javascript">
        var geocoder = new google.maps.Geocoder();

        function geocodePosition(pos) {
            geocoder.geocode({
                latLng: pos
            }, function (responses) {
                if (responses && responses.length > 0) {
                    updateMarkerAddress(responses[0].formatted_address);
                } else {
                    updateMarkerAddress('Cannot determine address at this location.');
                }
            });
        }


        function updateMarkerPosition(latLng) {

            document.getElementById('info').innerHTML = [
    latLng.lat(),
    latLng.lng()
  ].join(', ');

            document.getElementById('lblLocation').innerHTML = [
    latLng.lat(),
    latLng.lng()
  ].join(', ');

            document.getElementById('hfCoordinates').value = [
    latLng.lat(),
    latLng.lng()
  ].join(', ');

        }

        function updateMarkerAddress(str) {
            document.getElementById('address').innerHTML = str;
                       
        }




        function initialize() {
            var latLng = new google.maps.LatLng(41.547966, 21.800995);
            var mapOptions = {
                center: latLng,
                zoom: 8,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            var map = new google.maps.Map(document.getElementById("map-canvas"),
            mapOptions);
            var marker = new google.maps.Marker({
                position: latLng,
                title: 'Event Marker',
                map: map,
                draggable: true
            });
            // Update current position info.
            updateMarkerPosition(latLng);
            geocodePosition(latLng);

            // Add dragging event listeners.
            google.maps.event.addListener(marker, 'dragstart', function () {
                updateMarkerAddress('Dragging...');
            });

            google.maps.event.addListener(marker, 'drag', function () {
               // updateMarkerStatus('Dragging...');
                updateMarkerPosition(marker.getPosition());
            });

            google.maps.event.addListener(marker, 'dragend', function () {
              //  updateMarkerStatus('Drag ended');
                geocodePosition(marker.getPosition());
            });

          
          //end of initialize
        }

       google.maps.event.addDomListener(window, 'load', initialize);
    </script>
  







</head>
<body class=TextandBackGroundStyle>
<style>
  #map-canvas {
    width: 600px;
    height: 400px;
  
  }
  #infoPanel {
    float:none;
    margin-bottom: 10px;
  }
  #infoPanel div {
    margin-bottom: 5px;
  }
    .style1
    {
        width: 47px;
    }
  </style>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        
        

</asp:ToolkitScriptManager>
    <div style="height: 98px">
        
        <br />
        <table align=left >
     <td align=justify> <td align=left>
        <asp:Button ID="btnGoogleRevoke" runat="server" onclick="btnGoogleRevoke_Click" 
            Text="Revoke Google Calendar Rights" /></td>
         </td>   </table>
            <table align=right >
            
            <td align=right align=justify>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnLogOut" runat="server" onclick="btnLogOut_Click" 
            Text="LogOut" />
             </td></table>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
    
       <table align=center >
     <td align=justify> <td>
    
    </div>
    <asp:Button ID="btnNewEvent" runat="server" onclick="btnNewEvent_Click" 
        Text="CreateNew Event" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnPreviewEvents" runat="server" Text="Preview Events" 
        onclick="btnPreviewEvents_Click" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnEditEvents" runat="server" Text="EditMyEvents" 
        onclick="btnEditEvents_Click" />
        </td>
 </table>
    <br />
    <br />
    
    <table align=center >
     <td align=justify> <td>
        
    <asp:Panel ID="pnlPreviewEvents" runat="server">

    <iframe src="https://www.google.com/calendar/embed?src=<%=GetCalendarIdentifier()%>&ctz=Europe/Belgrade" style="border: 0" width="800"
height="600" frameborder="0" scrolling="no"></iframe>


    </asp:Panel>
    </td>
    </table>
    
        <table align=center >
         
         <td align=justify> <td><asp:Panel ID="pnlAddEvent" runat="server" Height="382px" Width="599px">
        
        <br />
        <br />
        Title:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtTitleEvent" runat="server" Height="16px" Width="419px"></asp:TextBox><br /><asp:RequiredFieldValidator ID="RequiredFieldValidatorTitle" runat="server" 
            ControlToValidate="txtTitleEvent" ErrorMessage="Event must contain Title" 
            ForeColor="#CC0000" validationgroup="btnSaveUpdate"></asp:RequiredFieldValidator>
        <br />
        Start Date :&nbsp;
        <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
        <asp:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" 
            TargetControlID="txtStartDate" PopupPosition=BottomLeft Format="MM/dd/yyyy">
        </asp:CalendarExtender>
        &nbsp;Start Time:&nbsp;&nbsp;&nbsp;&nbsp; HH :
        <asp:ListBox ID="lstboxStartHour" runat="server" Rows="1">
            <asp:ListItem Value="00 ">00</asp:ListItem>
            <asp:ListItem Value="01 ">01 </asp:ListItem>
            <asp:ListItem Value="02 ">02</asp:ListItem>
            <asp:ListItem Value="03 ">03</asp:ListItem>
            <asp:ListItem Value="04 ">04</asp:ListItem>
            <asp:ListItem Value="05 ">05</asp:ListItem>
            <asp:ListItem Value="06 ">06</asp:ListItem>
            <asp:ListItem Value="07 ">07</asp:ListItem>
            <asp:ListItem Value="08 ">08</asp:ListItem>
            <asp:ListItem Value="09 ">09</asp:ListItem>
            <asp:ListItem Value="10 ">10</asp:ListItem>
            <asp:ListItem Value="11 ">11 </asp:ListItem>
            <asp:ListItem Value="12 ">12</asp:ListItem>
            <asp:ListItem Value="13 ">13</asp:ListItem>
            <asp:ListItem Value="14 ">14</asp:ListItem>
            <asp:ListItem Value="15 ">15</asp:ListItem>
            <asp:ListItem Value="16 ">16</asp:ListItem>
            <asp:ListItem Value="17 ">17</asp:ListItem>
            <asp:ListItem Value="18 ">18</asp:ListItem>
            <asp:ListItem Value="19 ">19</asp:ListItem>
            <asp:ListItem Value="20 ">20</asp:ListItem>
            <asp:ListItem Value="21 ">21</asp:ListItem>
            <asp:ListItem Value="22 ">22</asp:ListItem>
            <asp:ListItem Value="23 ">23</asp:ListItem>
        </asp:ListBox>
        &nbsp; MM:&nbsp;&nbsp;
        <asp:ListBox ID="lstboxStartMinutes" runat="server" Rows="1">
          <asp:ListItem>00</asp:ListItem>
            <asp:ListItem>01</asp:ListItem>
            <asp:ListItem>02</asp:ListItem>
            <asp:ListItem>03</asp:ListItem>
            <asp:ListItem>04</asp:ListItem>
            <asp:ListItem>05</asp:ListItem>
            <asp:ListItem>06</asp:ListItem>
            <asp:ListItem>07</asp:ListItem>
            <asp:ListItem>08</asp:ListItem>
            <asp:ListItem>09</asp:ListItem>
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>11</asp:ListItem>
            <asp:ListItem>12</asp:ListItem>
            <asp:ListItem>13</asp:ListItem>
            <asp:ListItem>14</asp:ListItem>
            <asp:ListItem>15</asp:ListItem>
            <asp:ListItem>16</asp:ListItem>
            <asp:ListItem>17</asp:ListItem>
            <asp:ListItem>18</asp:ListItem>
            <asp:ListItem>19</asp:ListItem>
            <asp:ListItem>20</asp:ListItem>
            <asp:ListItem>21</asp:ListItem>
            <asp:ListItem>22</asp:ListItem>
            <asp:ListItem>23</asp:ListItem>
            <asp:ListItem>24</asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>26</asp:ListItem>
            <asp:ListItem>27</asp:ListItem>
            <asp:ListItem>28</asp:ListItem>
            <asp:ListItem>29</asp:ListItem>
            <asp:ListItem>30</asp:ListItem>
            <asp:ListItem>31</asp:ListItem>
            <asp:ListItem>32</asp:ListItem>
            <asp:ListItem>33</asp:ListItem>
            <asp:ListItem>34</asp:ListItem>
            <asp:ListItem>35</asp:ListItem>
            <asp:ListItem>36</asp:ListItem>
            <asp:ListItem>37</asp:ListItem>
            <asp:ListItem>38</asp:ListItem>
            <asp:ListItem>39</asp:ListItem>
            <asp:ListItem>40</asp:ListItem>
            <asp:ListItem>41</asp:ListItem>
            <asp:ListItem>42</asp:ListItem>
            <asp:ListItem>43</asp:ListItem>
            <asp:ListItem>44</asp:ListItem>
            <asp:ListItem>45</asp:ListItem>
            <asp:ListItem>46</asp:ListItem>
            <asp:ListItem>47</asp:ListItem>
            <asp:ListItem>48</asp:ListItem>
            <asp:ListItem>49</asp:ListItem>
            <asp:ListItem>50</asp:ListItem>
            <asp:ListItem>51</asp:ListItem>
            <asp:ListItem>52</asp:ListItem>
            <asp:ListItem>53</asp:ListItem>
            <asp:ListItem>54</asp:ListItem>
            <asp:ListItem>55</asp:ListItem>
            <asp:ListItem>56</asp:ListItem>
            <asp:ListItem>57</asp:ListItem>
            <asp:ListItem>58</asp:ListItem>
            <asp:ListItem>59</asp:ListItem>
        </asp:ListBox> <br /><asp:RequiredFieldValidator ID="RequiredFieldValidatorStartDate" runat="server" 
            ControlToValidate="txtStartDate" ErrorMessage="Event must contain Start Date" 
            ForeColor="#CC0000" validationgroup="btnSaveUpdate"></asp:RequiredFieldValidator>
        <br />
        End Date:&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
        <asp:CalendarExtender ID="txtEndDate_CalendatExtender" runat="server" 
            TargetControlID="txtEndDate" PopupPosition=BottomLeft Format="MM/dd/yyyy">
        </asp:CalendarExtender>
        &nbsp;End Time:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; HH:
        <asp:ListBox ID="lstboxEndHours" runat="server" Rows="1">
            <asp:ListItem Value="00">00</asp:ListItem>
            <asp:ListItem Value="01">01</asp:ListItem>
            <asp:ListItem Value="02 ">02</asp:ListItem>
            <asp:ListItem Value="03">03</asp:ListItem>
            <asp:ListItem Value="04 ">04</asp:ListItem>
            <asp:ListItem Value="05 ">05</asp:ListItem>
            <asp:ListItem Value="06 ">06</asp:ListItem>
            <asp:ListItem Value="07 ">07</asp:ListItem>
            <asp:ListItem Value="08 ">08</asp:ListItem>
            <asp:ListItem Value="09 ">09</asp:ListItem>
            <asp:ListItem Value="10 ">10</asp:ListItem>
            <asp:ListItem Value="11 ">11 </asp:ListItem>
            <asp:ListItem Value="12 ">12</asp:ListItem>
            <asp:ListItem Value="13 ">13</asp:ListItem>
            <asp:ListItem Value="14 ">14</asp:ListItem>
            <asp:ListItem Value="15 ">15</asp:ListItem>
            <asp:ListItem Value="16 ">16</asp:ListItem>
            <asp:ListItem Value="17 ">17</asp:ListItem>
            <asp:ListItem Value="18 ">18</asp:ListItem>
            <asp:ListItem Value="19 ">19</asp:ListItem>
            <asp:ListItem Value="20 ">20</asp:ListItem>
            <asp:ListItem Value="21 ">21</asp:ListItem>
            <asp:ListItem Value="22 ">22</asp:ListItem>
            <asp:ListItem Value="23 ">23</asp:ListItem>
        </asp:ListBox>
        &nbsp; MM:&nbsp;&nbsp;
        <asp:ListBox ID="lstboxEndMinutes0" runat="server" Rows="1">
            <asp:ListItem>00</asp:ListItem>
            <asp:ListItem>01</asp:ListItem>
            <asp:ListItem>02</asp:ListItem>
            <asp:ListItem>03</asp:ListItem>
            <asp:ListItem>04</asp:ListItem>
            <asp:ListItem>05</asp:ListItem>
            <asp:ListItem>06</asp:ListItem>
            <asp:ListItem>07</asp:ListItem>
            <asp:ListItem>08</asp:ListItem>
            <asp:ListItem>09</asp:ListItem>
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>11</asp:ListItem>
            <asp:ListItem>12</asp:ListItem>
            <asp:ListItem>13</asp:ListItem>
            <asp:ListItem>14</asp:ListItem>
            <asp:ListItem>15</asp:ListItem>
            <asp:ListItem>16</asp:ListItem>
            <asp:ListItem>17</asp:ListItem>
            <asp:ListItem>18</asp:ListItem>
            <asp:ListItem>19</asp:ListItem>
            <asp:ListItem>20</asp:ListItem>
            <asp:ListItem>21</asp:ListItem>
            <asp:ListItem>22</asp:ListItem>
            <asp:ListItem>23</asp:ListItem>
            <asp:ListItem>24</asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>26</asp:ListItem>
            <asp:ListItem>27</asp:ListItem>
            <asp:ListItem>28</asp:ListItem>
            <asp:ListItem>29</asp:ListItem>
            <asp:ListItem>30</asp:ListItem>
            <asp:ListItem>31</asp:ListItem>
            <asp:ListItem>32</asp:ListItem>
            <asp:ListItem>33</asp:ListItem>
            <asp:ListItem>34</asp:ListItem>
            <asp:ListItem>35</asp:ListItem>
            <asp:ListItem>36</asp:ListItem>
            <asp:ListItem>37</asp:ListItem>
            <asp:ListItem>38</asp:ListItem>
            <asp:ListItem>39</asp:ListItem>
            <asp:ListItem>40</asp:ListItem>
            <asp:ListItem>41</asp:ListItem>
            <asp:ListItem>42</asp:ListItem>
            <asp:ListItem>43</asp:ListItem>
            <asp:ListItem>44</asp:ListItem>
            <asp:ListItem>45</asp:ListItem>
            <asp:ListItem>46</asp:ListItem>
            <asp:ListItem>47</asp:ListItem>
            <asp:ListItem>48</asp:ListItem>
            <asp:ListItem>49</asp:ListItem>
            <asp:ListItem>50</asp:ListItem>
            <asp:ListItem>51</asp:ListItem>
            <asp:ListItem>52</asp:ListItem>
            <asp:ListItem>53</asp:ListItem>
            <asp:ListItem>54</asp:ListItem>
            <asp:ListItem>55</asp:ListItem>
            <asp:ListItem>56</asp:ListItem>
            <asp:ListItem>57</asp:ListItem>
            <asp:ListItem>58</asp:ListItem>
            <asp:ListItem>59</asp:ListItem>
        </asp:ListBox><br /><asp:RequiredFieldValidator ID="RequiredFieldValidatorEndDate" runat="server" 
            ControlToValidate="txtEndDate" ErrorMessage="Event must contain End Date" 
            ForeColor="#CC0000" validationgroup="btnSaveUpdate"></asp:RequiredFieldValidator>
            <br />
        <asp:Label ID="lblDateMessage" runat="server"></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="chkBoxLocation" runat="server" 
            oncheckedchanged="chkBoxLocation_CheckedChanged" Text="Add Location" AutoPostBack="True" />&nbsp;&nbsp;&nbsp;
            Coordinates: [&nbsp;<asp:Label ID="lblLocation" runat="server" Text="" ></asp:Label>&nbsp;]
             <asp:HiddenField ID="hfCoordinates" runat="server" />
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:CheckBox ID="chkBoxTransport" runat="server" 
            oncheckedchanged="chkBoxTransport_CheckedChanged" Text="Add Transport" AutoPostBack="True" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlTransport" runat="server" Width=100>
              <asp:ListItem Value="Walking">Walking</asp:ListItem>
        <asp:ListItem Value="Bicycle">Bicycle</asp:ListItem>
         <asp:ListItem Value="Car">Car</asp:ListItem>
          <asp:ListItem Value="Bus">Bus</asp:ListItem>
           <asp:ListItem Value="Airplane">Airplane</asp:ListItem>
             </asp:DropDownList> &nbsp;
              <br />
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  <asp:CheckBox ID="chkBoxWeather" runat="server" 
           Text="Include Weather" AutoPostBack="False" />
         <br />
        Remainders: 
             <asp:DropDownList ID="ddlRemainder1" runat="server" Width=100>
              <asp:ListItem Value="Email">Email</asp:ListItem>
        <asp:ListItem Value="Alert">Alert</asp:ListItem>
             </asp:DropDownList> &nbsp;
             <asp:TextBox ID="txtRemainder1" runat="server" Width=50></asp:TextBox>&nbsp; <asp:DropDownList ID="ddltimeRemainder1" runat="server" Width=100>
               <asp:ListItem Value="minutes">minutes</asp:ListItem>
              <asp:ListItem Value="hours">hours</asp:ListItem>
         <asp:ListItem Value="days">days</asp:ListItem>
           </asp:DropDownList> <asp:RangeValidator ID="RangeValidatorRemainder1" runat="server" Type="Integer" 
MinimumValue="0" MaximumValue="1000" ControlToValidate="txtRemainder1" 
ErrorMessage="Value must be a whole number between 0 and 1000" validationgroup="btnSaveUpdate"/>
      <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <asp:DropDownList ID="ddlRemainder2" runat="server" Width=100>
              <asp:ListItem Value="Email">Email</asp:ListItem>
        <asp:ListItem Value="Alert">Alert</asp:ListItem>
             </asp:DropDownList> &nbsp;
             <asp:TextBox ID="txtRemainder2" runat="server" Width=50></asp:TextBox>&nbsp; <asp:DropDownList ID="ddltimeRemainder2" runat="server" Width=100>
               <asp:ListItem Value="minutes">minutes</asp:ListItem>
              <asp:ListItem Value="hours">hours</asp:ListItem>
         <asp:ListItem Value="days">days</asp:ListItem>
          
             </asp:DropDownList><asp:RangeValidator ID="RangeValidatorRemainder2" runat="server" Type="Integer" 
MinimumValue="0" MaximumValue="1000" ControlToValidate="txtRemainder2" 
ErrorMessage="Value must be a whole number between 0 and 1000" validationgroup="btnSaveUpdate"/>

       <br />
        Description:<br />
        <asp:TextBox ID="txtEventDescription" runat="server" Height="130px" 
            TextMode="MultiLine" Width="400px"></asp:TextBox>
        <br />
        <asp:Label ID="lblErorEntryMessages" runat="server" Text=""></asp:Label>
        <asp:Label ID="lblSuccessfullUpdateSave" runat="server" Text=""></asp:Label>
        <br />             
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="btnSaveUpdate" runat="server" Text="Save/Update" 
                onclick="btnSaveUpdate_Click" validationgroup="btnSaveUpdate"  />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button 
                ID="btnClearAll" runat="server" Text="Clear Fields" 
                onclick="btnClearAll_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDeleteEvent" runat="server" onclick="btnDeleteEvent_Click" 
                Text="Delete Event" />
        <br />
        <br />
        <br />
        <br />
        <br />
    </asp:Panel></td> <td class="style1"> <asp:Panel ID="pnlMapEventLocation" runat="server"><div id="infoPanel">    
   <b> <i>Click and drag the marker.</i></b></div>
    <b>Current position:</b>
    <div id="info"></div>
    <b>Closest matching address:</b>
    <div id="address">
       
                </div>
   </div></asp:Panel>

    <asp:Panel ID="pnlMapPreview" runat="server" Height="382px" Width="600px">
            <div id="map-canvas"/>
            <br />
            <br />
            <br />
        </asp:Panel>
        
        </td></td> </table>
         <br /> <br /> <br /> <br /> <br />
         <br />
        <table> <td>
        
          <br />
          <br /> <asp:Panel ID="pnlEditGreedView" runat="server">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>My Events</b>
        
             <asp:GridView ID="gridViewEvent" runat="server" AutoGenerateColumns="False" 
                 BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                 CellPadding="3" onselectedindexchanged="gridViewEvent_SelectedIndexChanged" 
                    AllowPaging="True" onpageindexchanging="gridview_PageIndexChange">
                 <AlternatingRowStyle BackColor="#0066FF" ForeColor="White" />
                 <Columns>
                     <asp:CommandField ShowSelectButton="True" />
                     <asp:BoundField DataField="EventID" HeaderText="Event ID" />
                     <asp:BoundField DataField="EventTitle" HeaderText="Event Title" />
                     <asp:BoundField DataField="EventStartTime" HeaderText="Start Time" />
                     <asp:BoundField DataField="EventEndTime" HeaderText="End Time" />
                     <asp:BoundField DataField="EventDetails" HeaderText="Details" />
                     <asp:BoundField DataField="EventLocation" HeaderText="Location" />
                     <asp:BoundField DataField="EventRemainder1" HeaderText="Remainder1" />
                     <asp:BoundField DataField="EventRemainder2" HeaderText="Remainder2" />
                     <asp:BoundField DataField="EventTransport" HeaderText="Transport" />
                     <asp:BoundField DataField="Weather" HeaderText="Weather" />
                 </Columns>
                 <FooterStyle BackColor="White" ForeColor="#000066" />
                 <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                 <RowStyle ForeColor="#000066" />
                 <SelectedRowStyle BackColor="#FF9900" Font-Bold="True" ForeColor="White" />
                 <SortedAscendingCellStyle BackColor="#F1F1F1" />
                 <SortedAscendingHeaderStyle BackColor="#007DBB" />
                 <SortedDescendingCellStyle BackColor="#CAC9C9" />
                 <SortedDescendingHeaderStyle BackColor="#00547E" />
             </asp:GridView>
            </asp:Panel></td>
         </table>
         
          <br />
         <br />
         <br />

   
    </form>
   
</body>
</html>
