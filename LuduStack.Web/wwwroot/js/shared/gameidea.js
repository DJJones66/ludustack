﻿



var GAMEIDEA = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var interval = 120;
    var stopInterval = 1200;

    var genre = ["An action game", "An arcade game", "A top-down game", "An adventure game", "A strategy game", "RTS game", "A turn-based strategy game", "A role-playing game", "A platformer game", "A puzzle game", "A visual novel", "A social media game", "A mobile game", "A browser game", "An indie game", "An experimental game", "A student project", "An artsy game"];
    var allGenres = genre.length;
    var firstGenre = 0;

    var action = ['go to war with', 'wage war on', 'unite', 'lead', 'build', 'destroy', 'conquer', 'invade', 'colonize', 'discover', 'explore', 'trade with', 'lead the rebels in', 'make peace with', 'investigate', 'rename', 'collect gold from', 'collect crystals from', 'mine ore from', 'align', 'click on', 'match', 'throw', 'toss', 'fire pellets at', 'control', 'touch', 'stack', 'guess', 'memorize', 'rotate', 'swap', 'slide', 'avoid', 'drag and drop', 'tickle', 'race', 'challenge', 'collect', 'draw', 'unlock', 'cook', 'break', 'solve puzzles involving', 'collect', 'juggle'];
    var allActions = action.length;
    var firstAction = 0;

    var things = ['countries', 'nations', 'dragons', 'castles', 'cities', 'strongholds', 'towers', 'dungeons', 'citadels', 'kingdoms', 'unknown worlds', 'other worlds', 'parallel worlds', 'other dimensions', 'alien worlds', 'heaven', 'hell', 'mythological places', 'historical places', 'islands', 'sanctuaries', 'temples', 'ruins', 'factories', 'caves', 'gems', 'diamonds', 'gold nuggets', 'bricks', 'bubbles', 'squares', 'triangles', 'treasure', 'blobs', 'kitchen appliances', 'nondescript fruits', 'animals', 'birds', 'baby animals', 'farm animals', 'exotic fruits', 'sentient plants', 'your friends', 'shapes', 'jewels', 'letters', 'words', 'numbers', 'tokens', 'coins', 'eggs', 'hats', 'candy', 'chocolate', 'shoes', 'clothing items', 'princesses', 'blocks', 'cubes', 'asteroids', 'stars', 'balls', 'spheres', 'magnets', 'riddles'];
    var allThings = things.length;
    var firstThing = 0;

    var goals = ['to win', 'for glory', 'in the name of love', 'to live forever', 'to become the ruler of the world', 'to form an invincible empire', 'to win points', 'to reach the highscore', 'to unlock bonus items', 'to earn tokens', 'to unlock the next level'];
    var allGoals = goals.length;
    var firstGoal = 0;

    var pickingGenreInterval = 0, stopGenreInterval = 0;
    var pickingActionInterval = 0, stopActionInterval = 0;
    var pickingThingsInterval = 0, stopThingsInterval = 0;
    var pickingGoalsInterval = 0, stopGoalsInterval = 0;

    function setSelectors() {
        selectors.container = '#divGameIdea';
        selectors.btnGenerateGameIdea = '#btnGenerateGameIdea';
        selectors.genre = document.querySelector('.game-idea-genre');
        selectors.action = document.querySelector('.game-idea-action');
        selectors.things = document.querySelector('.game-idea-things');
        selectors.goals = document.querySelector('.game-idea-goals');
    }

    function cacheObjs() {
        objs.container = $(selectors.container);
    }

    function randomGenre() {
        selectors.genre.classList.add("picking");
        pickingGenreInterval = setInterval(changeGenre, interval);

        stopGenreInterval = setInterval(stopPigkingGenre, stopInterval);
    };

    function changeGenre() {
        selectors.genre.textContent = genre[firstGenre];
        firstGenre = (firstGenre + 1) % allGenres;
    }

    function stopPigkingGenre() {
        selectors.genre.classList.remove("picking");
        clearInterval(pickingGenreInterval);
        clearInterval(stopGenreInterval);
    };

    function randomAction() {
        selectors.action.classList.add("picking");
        pickingActionInterval = setInterval(changeAction, interval);

        stopActionInterval = setInterval(stopPigkingAction, stopInterval);
    };

    function changeAction() {
        selectors.action.textContent = action[firstAction];
        firstAction = (firstAction + 1) % allActions;
    }

    function stopPigkingAction() {
        selectors.action.classList.remove("picking");
        clearInterval(pickingActionInterval);
        clearInterval(stopActionInterval);
    };

    function randomThings() {
        selectors.things.classList.add("picking");
        pickingThingsInterval = setInterval(changeThings, interval);

        stopThingsInterval = setInterval(stopPigkingThings, stopInterval);
    };

    function changeThings() {
        selectors.things.textContent = things[firstThing];
        firstThing = (firstThing + 1) % allThings;
    }

    function stopPigkingThings() {
        selectors.things.classList.remove("picking");
        clearInterval(pickingThingsInterval);
        clearInterval(stopThingsInterval);
    };

    function randomGoals() {
        selectors.goals.classList.add("picking");
        pickingGoalsInterval = setInterval(changeGoals, interval);

        stopGoalsInterval = setInterval(stopPigkingGoals, stopInterval);
    };

    function changeGoals() {
        selectors.goals.textContent = goals[firstGoal];
        firstGoal = (firstGoal + 1) % allGoals;
    }

    function stopPigkingGoals() {
        selectors.goals.classList.remove("picking");
        clearInterval(pickingGoalsInterval);
        clearInterval(stopGoalsInterval);
    };

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        pick();
    }

    function bindAll() {
        bindGenerateClick();
    }

    function bindGenerateClick() {
        objs.container.on('click', selectors.btnGenerateGameIdea, function (e) {
            e.preventDefault();

            pick();

            return false;
        });
    }

    function pick() {
        randomGenre();
        randomAction();
        randomThings();
        randomGoals();
    }

    return {
        Init: init
    };
}());