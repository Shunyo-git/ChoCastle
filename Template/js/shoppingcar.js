//預載完成後執行項目
window.onload = function () {
    //畫面載入完成後點擊一下頁籤($('#defaultOpen').click();)
    document.getElementById("defaultOpen").click();
}

function buy() {
    var num = document.getElementById("innum").value;
    if (num <= 0) {
        alert("請選擇商品數量");
    } else {
        alert("商品已加入購物車囉!");
    }

}

function openPage(pageName, elmnt, color) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].style.backgroundColor = "";
    }
    document.getElementById(pageName).style.display = "block";
    elmnt.style.backgroundColor = color;
}

//增加數量
function AddNum() {
    var tmp = document.getElementById("innum");
    tmp.value = parseInt(tmp.value) + 1;
}

//減少數量
function LessNum() {
    var tmp = document.getElementById("innum");

    //判定數量是否小於0
    if (parseInt(tmp.value) - 1 < 0) {
        tmp.value = 0;
        alert("購買商品數量不得低於0個!");
    }
    else {
        tmp.value = parseInt(tmp.value) - 1;
    }
}