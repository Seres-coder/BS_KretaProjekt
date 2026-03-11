function toggleSidebar(){

	const sidebar = document.getElementById("sidebar");
	const content = document.querySelector(".content");

	sidebar.classList.toggle("closed");
	content.classList.toggle("full");

}

document.querySelectorAll(".nav-link").forEach(button => {
    button.addEventListener("click", function () {
        const targetId = this.getAttribute("data-view");

        document.querySelectorAll("section").forEach(section => {
            section.classList.remove("panel-active");
            section.classList.add("panel");
        });

        const targetPanel = document.getElementById(targetId);
        if (targetPanel) {
            targetPanel.classList.remove("panel");
            targetPanel.classList.add("panel-active");
        }
    });
});

function kuldes(){

let cimzett = document.getElementById("cimzett").value;
let tema = document.getElementById("tema").value;
let szoveg = document.getElementById("szoveg").value;

if(cimzett === "" || tema === "" || szoveg === ""){

document.getElementById("uzenetStatus").innerHTML =
"<span style='color:red'>Minden mezőt ki kell tölteni!</span>";

return;

}

document.getElementById("uzenetStatus").innerHTML =
"<span style='color:green'>Üzenet elküldve!</span>";

document.getElementById("cimzett").value = "";
document.getElementById("tema").value = "";
document.getElementById("szoveg").value = "";

}


function tanarmodositas() {
    document.querySelectorAll("#nev,#szak,#szuletes,#lakcim,#email")
        .forEach(input => input.disabled = false);

    document.getElementById("mentesGomb").style.display = "inline-block";
}

function tanarmentes() {
    document.querySelectorAll("#nev,#szak,#szuletes,#lakcim,#email")
        .forEach(input => input.disabled = true);

    document.getElementById("mentesGomb").style.display = "none";
}

function diakmodositas() {
    document.querySelectorAll("#nev,#osztaly,#szuletes,#lakcim,#email,#szulo")
        .forEach(input => input.disabled = false);

    document.getElementById("mentesGomb").style.display = "inline-block";
}

function diakmentes() {
    document.querySelectorAll("#nev,#osztaly,#szuletes,#lakcim,#email,#szulo")
        .forEach(input => input.disabled = true);

    document.getElementById("mentesGomb").style.display = "none";
}


const orarendAdatok = {
    "9.A": {
        hetfo:    ["Matek", "Magyar", "Angol", "Töri", "Info", "Tesnevelés", ""],
        kedd:     ["Fizika", "Matek", "Angol", "Biológia", "Irodalom", "", ""],
        szerda:   ["Kémia", "Matek", "Info", "Töri", "Angol", "Osztályfőnöki", ""],
        csutortok:["Magyar", "Fizika", "Matek", "Földrajz", "Tesnevelés", "", ""],
        pentek:   ["Angol", "Irodalom", "Matek", "Info", "Biológia", "", ""]
    },
    "10.A": {
        hetfo:    ["Matek", "Töri", "Angol", "Info", "Fizika", "", ""],
        kedd:     ["Magyar", "Matek", "Biológia", "Tesnevelés", "Angol", "", ""],
        szerda:   ["Kémia", "Info", "Matek", "Töri", "Irodalom", "", ""],
        csutortok:["Fizika", "Matek", "Angol", "Földrajz", "Magyar", "", ""],
        pentek:   ["Info", "Matek", "Biológia", "Osztályfőnöki", "Tesnevelés", "", ""]
    },
    "11.B": {
        hetfo:    ["Irodalom", "Matek", "Angol", "Töri", "Info", "", ""],
        kedd:     ["Fizika", "Kémia", "Matek", "Magyar", "Angol", "", ""],
        szerda:   ["Biológia", "Matek", "Töri", "Info", "Tesnevelés", "", ""],
        csutortok:["Angol", "Matek", "Fizika", "Irodalom", "Földrajz", "", ""],
        pentek:   ["Magyar", "Info", "Matek", "Osztályfőnöki", "Tesnevelés", "", ""]
    }
};

document.addEventListener("DOMContentLoaded", function () {
    const navButtons = document.querySelectorAll(".nav-link");

    navButtons.forEach(button => {
        button.addEventListener("click", function () {
            const targetId = this.getAttribute("data-view");

            document.querySelectorAll("section").forEach(section => {
                section.classList.remove("panel-active");
                section.classList.add("panel");
            });

            const targetPanel = document.getElementById(targetId);
            if (targetPanel) {
                targetPanel.classList.remove("panel");
                targetPanel.classList.add("panel-active");
            }
        });
    });

    osztalyokBetoltese();
});

function osztalyokBetoltese() {
    const select = document.getElementById("osztalySelect");
    if (!select) return;

    select.innerHTML = '<option value="">Válassz osztályt</option>';

    Object.keys(orarendAdatok).forEach(osztaly => {
        const option = document.createElement("option");
        option.value = osztaly;
        option.textContent = osztaly;
        select.appendChild(option);
    });
}


function orarendBetoltes() {
    const select = document.getElementById("osztalySelect");
    const container = document.getElementById("orarendContainer");
    const osztaly = select.value;

    if (!osztaly) {
        container.innerHTML = '<p class="text-danger">Kérlek válassz egy osztályt.</p>';
        return;
    }

    const adat = orarendAdatok[osztaly];

    if (!adat) {
        container.innerHTML = '<p class="text-danger">Ehhez az osztályhoz nincs órarend.</p>';
        return;
    }

    const oraSzamok = [1, 2, 3, 4, 5, 6, 7];
    const napok = [
        { key: "hetfo", label: "Hétfő" },
        { key: "kedd", label: "Kedd" },
        { key: "szerda", label: "Szerda" },
        { key: "csutortok", label: "Csütörtök" },
        { key: "pentek", label: "Péntek" }
    ];

    let html = `
        <h4 class="mb-3">${osztaly} osztály órarendje</h4>
        <div class="table-responsive">
            <table class="table table-bordered table-striped text-center align-middle">
                <thead class="table-dark">
                    <tr>
                        <th>Óra</th>
    `;

    napok.forEach(nap => {
        html += `<th>${nap.label}</th>`;
    });

    html += `
                    </tr>
                </thead>
                <tbody>
    `;

    oraSzamok.forEach((ora, index) => {
        html += `<tr><td><b>${ora}.</b></td>`;

        napok.forEach(nap => {
            const tantargy = adat[nap.key][index] || "-";
            html += `<td>${tantargy === "" ? "-" : tantargy}</td>`;
        });

        html += `</tr>`;
    });

    html += `
                </tbody>
            </table>
        </div>
    `;

    container.innerHTML = html;
}


function osztalyokBetoltese() {
    const select1 = document.getElementById("osztalySelect");
    const select2 = document.getElementById("modOsztalySelect");

    if (select1) {
        select1.innerHTML = '<option value="">Válassz osztályt</option>';
    }

    if (select2) {
        select2.innerHTML = '<option value="">Válassz osztályt</option>';
    }

    Object.keys(orarendAdatok).forEach(osztaly => {
        if (select1) {
            const option1 = document.createElement("option");
            option1.value = osztaly;
            option1.textContent = osztaly;
            select1.appendChild(option1);
        }

        if (select2) {
            const option2 = document.createElement("option");
            option2.value = osztaly;
            option2.textContent = osztaly;
            select2.appendChild(option2);
        }
    });
}

function szerkeszthetoOrarendBetoltes() {
    const select = document.getElementById("modOsztalySelect");
    const container = document.getElementById("orarendModContainer");
    const osztaly = select.value;

    if (!osztaly) {
        container.innerHTML = '<p class="text-danger">Kérlek válassz egy osztályt.</p>';
        return;
    }

    const adat = orarendAdatok[osztaly];

    if (!adat) {
        container.innerHTML = '<p class="text-danger">Ehhez az osztályhoz nincs órarend.</p>';
        return;
    }

    const oraSzamok = [1, 2, 3, 4, 5, 6, 7];
    const napok = [
        { key: "hetfo", label: "Hétfő" },
        { key: "kedd", label: "Kedd" },
        { key: "szerda", label: "Szerda" },
        { key: "csutortok", label: "Csütörtök" },
        { key: "pentek", label: "Péntek" }
    ];

    let html = `
        <h4 class="mb-3">${osztaly} osztály órarendjének módosítása</h4>
        <div class="table-responsive">
            <table class="table table-bordered table-striped text-center align-middle">
                <thead class="table-dark">
                    <tr>
                        <th>Óra</th>
    `;

    napok.forEach(nap => {
        html += `<th>${nap.label}</th>`;
    });

    html += `</tr></thead><tbody>`;

    oraSzamok.forEach((ora, index) => {
        html += `<tr><td><b>${ora}.</b></td>`;

        napok.forEach(nap => {
            const tantargy = adat[nap.key][index] || "";
            html += `
                <td>
                    <input 
                        type="text" 
                        class="form-control"
                        value="${tantargy}"
                        data-nap="${nap.key}"
                        data-oraindex="${index}"
                    >
                </td>
            `;
        });

        html += `</tr>`;
    });

    html += `
                </tbody>
            </table>
        </div>

        <button class="btn btn-success mt-3" onclick="orarendModositasMentese()">
            Mentés
        </button>

        <p id="orarendMentesStatus" class="mt-3"></p>
    `;

    container.innerHTML = html;
}

function orarendModositasMentese() {
    const osztaly = document.getElementById("modOsztalySelect").value;
    const status = document.getElementById("orarendMentesStatus");

    if (!osztaly || !orarendAdatok[osztaly]) {
        status.innerHTML = '<span class="text-danger">Nincs kiválasztott osztály.</span>';
        return;
    }

    const inputs = document.querySelectorAll("#orarendModContainer input[data-nap][data-oraindex]");

    const ujAdat = {
        hetfo: ["", "", "", "", "", "", ""],
        kedd: ["", "", "", "", "", "", ""],
        szerda: ["", "", "", "", "", "", ""],
        csutortok: ["", "", "", "", "", "", ""],
        pentek: ["", "", "", "", "", "", ""]
    };

    inputs.forEach(input => {
        const nap = input.getAttribute("data-nap");
        const oraIndex = parseInt(input.getAttribute("data-oraindex"));
        ujAdat[nap][oraIndex] = input.value.trim();
    });

    orarendAdatok[osztaly] = ujAdat;

    status.innerHTML = '<span class="text-success">Az órarend sikeresen módosítva lett.</span>';
}