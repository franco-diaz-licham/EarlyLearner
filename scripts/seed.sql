begin;

insert into households (id, name, created_on)
values
    ('11111111-1111-1111-1111-111111111111', 'Demo Household', now())
on conflict (id) do nothing;

insert into users (id, email, external_object_id, external_tenant_id, first_name, last_name, status, created_on)
values
    ('33333333-3333-3333-3333-333333333331', 'avery.taylor@example.local', 'demo-external-object-avery-taylor', 'demo-external-tenant', 'Avery', 'Taylor', 'Active', now()),
    ('33333333-3333-3333-3333-333333333332', 'jordan.taylor@example.local', 'demo-external-object-jordan-taylor', 'demo-external-tenant', 'Jordan', 'Taylor', 'Active', now())
on conflict (id) do update set
    email = excluded.email,
    external_object_id = excluded.external_object_id,
    external_tenant_id = excluded.external_tenant_id,
    first_name = excluded.first_name,
    last_name = excluded.last_name,
    status = excluded.status;

insert into carers (id, household_id, user_id, role, created_on)
values
    ('22222222-2222-2222-2222-222222222221', '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333331', 'Owner', now()),
    ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333332', 'Caregiver', now())
on conflict (id) do nothing;

insert into children (id, household_id, first_name, last_name, date_of_birth, is_archived, created_on)
values
    ('44444444-4444-4444-4444-444444444441', '11111111-1111-1111-1111-111111111111', 'Mia', 'Taylor', '2021-03-14', false, now()),
    ('44444444-4444-4444-4444-444444444442', '11111111-1111-1111-1111-111111111111', 'Noah', 'Taylor', '2022-08-02', false, now())
on conflict (id) do nothing;

insert into learning_outcomes (id, code, name, description, category, sort_order, status, created_on)
values
    ('55555555-5555-5555-5555-555555555551', 'language-listening', 'Listens and responds', 'Listens to short instructions and responds with words or actions.', 'Language', 10, 'Active', now()),
    ('55555555-5555-5555-5555-555555555552', 'social-turn-taking', 'Takes turns with others', 'Practises waiting, sharing, and taking turns during play.', 'Social', 20, 'Active', now()),
    ('55555555-5555-5555-5555-555555555553', 'motor-pencil-grip', 'Uses early mark making', 'Uses crayons, pencils, or brushes to make controlled marks.', 'Motor', 30, 'Active', now())
on conflict (id) do nothing;

insert into daily_logs (id, household_id, child_id, log_date, created_on)
values
    ('99999999-9999-9999-9999-999999999991', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444441', '2026-06-17', now())
on conflict (id) do nothing;

insert into learning_moments (id, daily_log_id, kind, title, notes, created_on)
values
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '99999999-9999-9999-9999-999999999991', 'Activity', 'Played a matching game and waited for turns', 'Mia waited for her turn twice and celebrated when Jordan had a turn.', now()),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '99999999-9999-9999-9999-999999999991', 'Reading', 'Read Brown Bear, Brown Bear, What Do You See?', 'Named animals and repeated some colour words.', now()),
    ('cccccccc-cccc-cccc-cccc-ccccccccccc1', '99999999-9999-9999-9999-999999999991', 'Routine', 'Practised the morning routine', 'Put shoes near the door and followed two simple steps with one reminder.', now())
on conflict (id) do nothing;

insert into learning_moment_learning_outcomes (learning_moment_id, learning_outcome_id)
values
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '55555555-5555-5555-5555-555555555552'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '55555555-5555-5555-5555-555555555551'),
    ('cccccccc-cccc-cccc-cccc-ccccccccccc1', '55555555-5555-5555-5555-555555555553')
on conflict (learning_moment_id, learning_outcome_id) do nothing;

commit;
