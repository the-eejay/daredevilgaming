import random

players = []

backstabbers = []
cooperators = []

weighted_death_list = []

reward = 0.0
round_num = 0

class Player:
    action = ''
    gold = 0
    alive = True

    def backstab(self):
        self.action = 'b'
        print('backstab\n')

    def cooperate(self):
        self.action = 'c'
        print('cooperate\n')

    def add_gold(self, gold):
        self.gold += gold

    def act_randomly(self):
        if random.random() < 0.5:
            self.backstab()
        else:
            self.cooperate()
            
    def get_action(self):
        return self.action

    def get_score(self):
        return self.gold

    def kill(self):
        self.alive = False

    def revive(self):
        self.alive = True

    def is_alive(self):
        return self.alive

def main():

    print_greeting()
    
    for i in range(6):
        x = Player()
        players.append(x)
    game_loop()

def game_loop():
    global reward, round_num, cooperators, backstabbers, weighted_death_list

    reward += 120.0

    round_num += 1
    
    print_scoreboard()
    
    player_in = raw_input('Press c to co-operate, or b to backstab\n')

    if player_in == 'c': players[0].cooperate()
    else: players[0].backstab()
    
    for i in range(1,6):
        print('Player ' + str(i+1) + '\'s action:')
        players[i].act_randomly()

    for p in players:
        p.revive()
        if p.get_action() == 'b':
            backstabbers.append(p)
        else:
            cooperators.append(p)

    kill_players()

    survivors = []
    
    for p in players:
        if p.is_alive():
            survivors.append(p)

    if mission_passed(len(survivors)):
        for p in survivors:
            p.add_gold(reward/float(len(survivors)))

    weighted_death_list = []
    backstabbers = []
    cooperators = []

    if round_num < 10:
        game_loop()
    else:
        finish()

def kill_players():
    weighted_death_list.extend(cooperators)
    weighted_death_list.extend(backstabbers)
    weighted_death_list.extend(backstabbers)

    for p in players:
        if p.get_action() == 'b':
            random.choice(weighted_death_list).kill()

def print_greeting():
    print('Welcome! You are up against 5 random players and are attempting' +
          ' to maximise your gold score.\n')
    
def finish():
    print('The scores are final!')
    print_scoreboard()

def print_scoreboard():
    i = 1
    print('Here are the scores.')
    for p in players:
        print('Player ' + str(i) + ' score: ' + str(p.get_score()) + ' (alive: ' + str(p.is_alive()) + ')')
        i += 1
    print('\n')

def mission_passed(num_survivors):
    if random.randint(1, 6) <= num_survivors:
        print('Mission passed!')
        return True
    print('Mission failed!')
    return False

main()
