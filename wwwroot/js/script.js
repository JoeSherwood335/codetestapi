const url = "http://localhost:5000/houseing";
const updateurl = "http://localhost:5000/houseing?update=t"
const d = new Date();

function button_onclick()
{
  console.log("getdata_onclick");

  updatedata(updateurl);
}

function updatedata(source){
  fetch(source)
  .then((res) => res.json())
  .then((data) => {
    console.log("fetch data");

    let s = data.parcelid.source[0];
  
    content.innerHTML = `<p> ${data.parcelid.id} downloaded on ${d.getMonth()}-${d.getDate()} and updated on ${s.record.PARCEL.update_date} </p>`;       
         
    content.insertAdjacentHTML("beforeend", `<dl> \
            <dt>${s.record.neighbor.description}</dt> \
            <dd>${s.record.neighbor.value}</dd> \
        </dl>`);

    content.insertAdjacentHTML("beforeend", `<dl> \
            <dt>Address</dt> \
            <dd>${s.record.number.value} ${s.record.street.value} ${s.record.MAIL_CITY.value} ${s.record.MAIL_STATE.value} ${s.record.MAIL_ZIPCODE.value}</dd> \
        </dl>`);

    console.log("2");
    
  }).catch(function(error){
    console.log(`error message ${error}`);
  });
}

let content = document.getElementById('content');

document.getElementById('RefreshData').addEventListener('click', button_onclick);

function ready(callbackFunction){
  if(document.readyState != 'loading')
    callbackFunction()
  else
    document.addEventListener("DOMContentLoaded", callbackFunction)
}
ready(event => {
  //alert("hello")
  updatedata(url);
});

let map;

function initMap() {
  map = new google.maps.Map(document.getElementById("map"), {
    center: { lat: -34.397, lng: 150.644 },
    zoom: 8,
  });
}

initMap();


