import jsdom = require("jsdom")

const url = "https://csc375-open-source-development.github.io/MyQ-Course-Photo-Roster-Page/"

let main = async () => {
    let results: {
        name: string
    }[] = []
    let req = await fetch(url, {
        method: "GET"
    })
    if (!req.ok) {
        console.error("Unable to connect to the website")
        process.exit(1)
    }
    const dom = new jsdom.JSDOM(await req.text(), {
        url
    })
    let cards = [...dom.window.document.querySelectorAll('.student-card div.student-info')]
    for (let card of cards) {
        let raw_name = card.querySelector("h2").textContent
        let name = raw_name
        if (raw_name.includes(", ")) {
            name = raw_name.split(", ").reverse().join(' ')
        }
        results.push({
            name
        })
    }
    results.sort((a, b) => {
        if (a.name < b.name) {
            return -1
        }

        if (a.name > b.name) {
            return 1
        } 

        return 0;
    })
    console.log(results.map(v => v.name + "\n").join(''))
}

main();

